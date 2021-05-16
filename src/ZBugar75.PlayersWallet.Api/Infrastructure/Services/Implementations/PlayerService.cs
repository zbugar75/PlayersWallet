﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly IPlayerRepository _players;
        private readonly IWalletRepository _wallets;
        private readonly ITransactionRepository _transactions;
        private readonly ITransactionResponseCacheRepository _transactionResponseCache;

        private static AsyncReaderWriterLock padlock = new AsyncReaderWriterLock();

        public PlayerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _players = unitOfWork.Players;
            _wallets = unitOfWork.Wallets;
            _transactions = _unitOfWork.Transactions;
            _transactionResponseCache = unitOfWork.TransactionResponseCache;
        }

        public async Task<Player> CreatePlayerAsync(string username, CancellationToken cancellationToken)
        {
            if (username is null || username == string.Empty)
                throw new ArgumentNullException(nameof(username));

            if (await _players.ExistsPlayerWithUsernameAsync(username, cancellationToken).ConfigureAwait(false))
                throw new DuplicateEntityException($"Duplicate exception. Username '{username}' already exists.");

            var player = await _players
                .AddAsync(new Player { Username = username }, cancellationToken)
                .ConfigureAwait(false);

            await _wallets
                .AddAsync(new Wallet { PlayerId = player.Id, Balance = 0 }, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

            return player;
        }

        public async Task<Wallet> GetBalanceAsync(Guid playerId, CancellationToken cancellationToken)
        {
            await EnsurePlayerExistsAsync(playerId, cancellationToken).ConfigureAwait(false);

            var wallet = await _wallets.GetExistingAsync(playerId, cancellationToken).ConfigureAwait(false);

            if (wallet == null)
                throw new EntityNotFoundException($"Entity not found exception. Wallet for player {playerId} not found.");

            return wallet;
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(CancellationToken cancellationToken)
        {
            return await _players.GetAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid playerId, CancellationToken cancellationToken)
        {
            await EnsurePlayerExistsAsync(playerId, cancellationToken).ConfigureAwait(false);

            return await _transactions.GetAllForPlayerAsync(playerId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TransactionResponse> RegisterTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            await EnsurePlayerExistsAsync(transaction.PlayerId, cancellationToken).ConfigureAwait(false);

            using (await padlock.WriterLockAsync(cancellationToken).ConfigureAwait(false))
            {
                var transactionResponse = await _transactionResponseCache
                    .GetAsync(transaction.PlayerId, cancellationToken)
                    .ConfigureAwait(false);

                if (transactionResponse is not null)
                    return transactionResponse;

                try
                {
                    // Db transaction is not supported on InMemory Database
                    // I think, this step would be correct with a db transaction
                    // await _unitOfWork.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                    var wallet = await _wallets
                        .GetExistingAsync(transaction.PlayerId, cancellationToken)
                        .ConfigureAwait(false);

                    if (transaction.TransactionType == TransactionType.Stake && transaction.Amount > wallet.Balance)
                        throw new LowBalanceException($"Low balance exception for the Player {transaction.PlayerId}");

                    if (transaction.TransactionType == TransactionType.Stake)
                    {
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
                    //await _unitOfWork.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);

                    transactionResponse = new TransactionResponse
                    {
                        TransactionId = transaction.Id,
                        ResponseStatusCode = StatusCodes.Status403Forbidden
                    };

                    await _transactionResponseCache
                        .AddAsync(transactionResponse, cancellationToken)
                        .ConfigureAwait(false);

                    await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
                }

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
