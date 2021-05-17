using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Zbugar75.PlayersWallet.Api.Common.Exceptions;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Domain.Entities.Enums;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;
using Zbugar75.PlayersWallet.Api.Infrastructure.UnitOfWork;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Services.Implementations
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlayerService> _logger;
        private readonly IPlayerRepository _players;
        private readonly IWalletRepository _wallets;
        private readonly ITransactionRepository _transactions;
        private readonly ITransactionResponseCacheRepository _transactionResponseCache;

        private static AsyncReaderWriterLock padlock = new AsyncReaderWriterLock();

        public PlayerService(IUnitOfWork unitOfWork, ILogger<PlayerService> _logger)
        {
            _unitOfWork = unitOfWork;
            this._logger = _logger;
            _players = unitOfWork.Players;
            _wallets = unitOfWork.Wallets;
            _transactions = _unitOfWork.Transactions;
            _transactionResponseCache = unitOfWork.TransactionResponseCache;
        }

        public async Task<Player> CreatePlayerAsync(string username, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{method} started for {username}.", nameof(CreatePlayerAsync), username);
            if (username is null || username == string.Empty)
                throw new ArgumentNullException(nameof(username));

            using (await padlock.WriterLockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await _players.ExistsPlayerWithUsernameAsync(username, cancellationToken).ConfigureAwait(false))
                    throw new DuplicateEntityException($"Duplicate exception. Username '{username}' already exists.");

                var player = await _players
                    .AddAsync(new Player { Username = username }, cancellationToken)
                    .ConfigureAwait(false);

                await _wallets
                    .AddAsync(new Wallet { PlayerId = player.Id, Balance = 0 }, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

                _logger.LogDebug("{method} finished. Player with {playerId} id created.", nameof(CreatePlayerAsync), player.Id);
                return player;
            }
        }

        public async Task<Wallet> GetBalanceAsync(Guid playerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{method} started for {playerId}.", nameof(GetBalanceAsync), playerId);
            using (await padlock.ReaderLockAsync(cancellationToken).ConfigureAwait(false))
            {
                await EnsurePlayerExistsAsync(playerId, cancellationToken).ConfigureAwait(false);

                var wallet = await _wallets.GetExistingAsync(playerId, cancellationToken).ConfigureAwait(false);

                if (wallet == null)
                    throw new EntityNotFoundException(
                        $"Entity not found exception. Wallet for player {playerId} not found.");

                _logger.LogDebug("{method} executed for {playerId}. Balance {balance} retrieved.", nameof(GetBalanceAsync), playerId, wallet.Balance);
                return wallet;
            }
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("{method} started.", nameof(GetPlayersAsync));
            using (await padlock.ReaderLockAsync(cancellationToken).ConfigureAwait(false))
            {
                return await _players.GetAllAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid playerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{method} started.", nameof(GetTransactionsAsync));
            using (await padlock.ReaderLockAsync(cancellationToken).ConfigureAwait(false))
            {
                await EnsurePlayerExistsAsync(playerId, cancellationToken).ConfigureAwait(false);

                return await _transactions.GetAllForPlayerAsync(playerId, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<TransactionResponse> RegisterTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{method} started for transaction {transactionId}.", nameof(RegisterTransactionAsync), transaction.Id);
            using (await padlock.ReaderLockAsync(cancellationToken).ConfigureAwait(false))
            {
                await EnsurePlayerExistsAsync(transaction.PlayerId, cancellationToken).ConfigureAwait(false);
            }

            _logger.LogTrace("{method}: Player {playerId} found.", nameof(RegisterTransactionAsync), transaction.PlayerId);
            using (await padlock.WriterLockAsync(cancellationToken).ConfigureAwait(false))
            {
                var transactionResponse = await _transactionResponseCache
                    .GetAsync(transaction.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (transactionResponse is not null)
                {
                    _logger.LogTrace("{method}: Transaction found in cache. Returning response.", nameof(RegisterTransactionAsync), transaction.PlayerId);
                    return transactionResponse;
                }

                try
                {
                    _logger.LogTrace("{method}: Transaction not found in cache. Returning response.", nameof(RegisterTransactionAsync), transaction.PlayerId);
                    var wallet = await _wallets
                        .GetExistingAsync(transaction.PlayerId, cancellationToken)
                        .ConfigureAwait(false);


                    if (transaction.TransactionType == TransactionType.Stake)
                    {
                        if (transaction.Amount > wallet.Balance)
                        {
                            _logger.LogTrace("{method}: LowBalanceException. Transaction {transactionId} will be as rejected registered.", nameof(RegisterTransactionAsync), transaction.PlayerId);
                            throw new LowBalanceException($"Low balance exception for the Player {transaction.PlayerId}");
                        }

                        wallet.Balance -= transaction.Amount;
                    }
                    else
                    {
                        wallet.Balance += transaction.Amount;
                    }

                    transactionResponse = new TransactionResponse
                    {
                        TransactionId = transaction.Id,
                        ResponseStatusCode = StatusCodes.Status202Accepted
                    };

                    await _transactionResponseCache
                        .AddAsync(transactionResponse, cancellationToken)
                        .ConfigureAwait(false);

                    await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogTrace("{method}: Transaction {transactionId} registered successfully. The new balance is {balance}.", nameof(RegisterTransactionAsync), transaction.PlayerId, wallet.Balance);
                }
                catch (Exception)
                {
                    transactionResponse = new TransactionResponse
                    {
                        TransactionId = transaction.Id,
                        ResponseStatusCode = StatusCodes.Status403Forbidden
                    };

                    await _transactionResponseCache
                        .AddAsync(transactionResponse, cancellationToken)
                        .ConfigureAwait(false);

                    await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogTrace("{method}: Transaction {transactionId} was not executed.", nameof(RegisterTransactionAsync), transaction.PlayerId);
                }

                _logger.LogDebug("{method} finished for transaction {transactionId}.", nameof(RegisterTransactionAsync), transaction.Id);
                return transactionResponse;
            }
        }

        private async Task EnsurePlayerExistsAsync(Guid playerId, CancellationToken cancellationToken)
        {
            if (!await _players.ExistsPlayerWithPlayerIdAsync(playerId, cancellationToken).ConfigureAwait(false))
                throw new EntityNotFoundException($"Entity not found exception. Player {playerId} not found.");
        }
    }
}
