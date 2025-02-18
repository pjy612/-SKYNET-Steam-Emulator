﻿using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks
{
    [StructLayout(LayoutKind.Sequential)]
    public class CSteamID : IEquatable<CSteamID>, IComparable<CSteamID>, IEquatable<ulong>, IComparable<ulong>
    {
        public ulong SteamID;
        public uint AccountID;
        public byte Universe;
        public byte AccountType;

        public static CSteamID Invalid = (CSteamID)0;

        public CSteamID(uint accountId)
        {
            this.AccountID = accountId;
            this.Universe = (byte)EUniverse.k_EUniversePublic;
            this.AccountType = (byte)EAccountType.k_EAccountTypeIndividual;

            int instance = (AccountType == (byte)EAccountType.k_EAccountTypeClan || AccountType == (byte)EAccountType.k_EAccountTypeGameServer) ? 0 : 1;

            this.SteamID = 0;
            this.SteamID = (SteamID & ~(0xFFFFFFFFul << (ushort)0)) | (((ulong)(AccountID) & 0xFFFFFFFFul) << (ushort)0);
            this.SteamID = (SteamID & ~(0xFFul << (ushort)56)) | (((ulong)(Universe) & 0xFFul) << (ushort)56);
            this.SteamID = (SteamID & ~(0xFul << (ushort)52)) | (((ulong)(AccountType) & 0xFul) << (ushort)52);
            this.SteamID = (SteamID & ~(0xFFFFFul << (ushort)32)) | (((ulong)(instance) & 0xFFFFFul) << (ushort)32);
        }

        public CSteamID(ulong _steamID)
        {
            this.SteamID = _steamID;
            this.AccountID = (uint)(_steamID & 0xFFFFFFFFul);
            this.Universe = (byte)(EUniverse)((_steamID >> 56) & 0xFFul);
            this.AccountType = (byte)(EAccountType)((_steamID >> 52) & 0xFul);
        }

        public CSteamID(uint _accountId, EUniverse _Universe, EAccountType _AccountType)
        {
            this.AccountID = _accountId;
            this.Universe = (byte)_Universe;
            this.AccountType = (byte)_AccountType;

            int instance = (_AccountType == EAccountType.k_EAccountTypeClan || _AccountType == EAccountType.k_EAccountTypeGameServer) ? 0 : 1;

            this.SteamID = 0;
            this.SteamID = (SteamID & ~(0xFFFFFFFFul << (ushort)0)) | (((ulong)(_accountId) & 0xFFFFFFFFul) << (ushort)0);
            this.SteamID = (SteamID & ~(0xFFul << (ushort)56)) | (((ulong)(_Universe) & 0xFFul) << (ushort)56);
            this.SteamID = (SteamID & ~(0xFul << (ushort)52)) | (((ulong)(_AccountType) & 0xFul) << (ushort)52);
            this.SteamID = (SteamID & ~(0xFFFFFul << (ushort)32)) | (((ulong)(instance) & 0xFFFFFul) << (ushort)32);
        }

        public static CSteamID CreateOne()
        {
            CSteamID randomID = new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
            return randomID;
        }

        public override int GetHashCode()
        {
            return this.SteamID.GetHashCode();
        }

        public int CompareTo(CSteamID other)
        {
            return this.SteamID.CompareTo(other.SteamID);
        }

        public bool Equals(CSteamID other)
        {
            return this.SteamID == other.SteamID;
        }

        public static bool operator ==(CSteamID x, CSteamID y)
        {
            return x.SteamID == y.SteamID;
        }

        public static bool operator !=(CSteamID x, CSteamID y)
        {
            return !(x.SteamID == y.SteamID);
        }

        public static explicit operator CSteamID(ulong value)
        {
            return new CSteamID(value);
        }

        public static explicit operator ulong(CSteamID that)
        {
            return that.SteamID;
        }

        public override string ToString()
        {
            return $"[U:{Universe}:{AccountID}] ({SteamID})";
        }

        public static explicit operator string(CSteamID that)
        {
            return (string)that;
        }

        public bool Equals(ulong other)
        {
            return this.SteamID == other;
        }

        public int CompareTo(ulong other)
        {
            return this.SteamID.CompareTo(other);
        }

        public static bool operator ==(CSteamID x, ulong y)
        {
            return x.SteamID == y;
        }

        public static bool operator !=(CSteamID x, ulong y)
        {
            return !(x.SteamID == y);
        }

        public static bool operator ==(ulong y, CSteamID x)
        {
            return x.SteamID == y;
        }

        public static bool operator !=(ulong y, CSteamID x)
        {
            return !(x.SteamID == y);
        }

        public static CSteamID GenerateGameServer()
        {
            return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeGameServer);
        }

        public static CSteamID CreateUnauthenticatedUser()
        {
            return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeAnonUser);
        }
    }
}

