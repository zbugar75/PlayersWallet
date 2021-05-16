﻿using System;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared.Extensions
{
    public static class IntegerExtensions
    {
        public static Guid ToGuid(this int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}