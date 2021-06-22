﻿using System;
namespace BookStore_API.Contracts
{
    public interface iLoggerService
    {
        void LogInfo(string message);

        void LogWarn(string message);

        void LogDebug(string message);

        void LogError(string message);

    }
}
