﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using frɪdʒ.utils;

namespace frɪdʒ.Db
{
	internal static class Users
	{
		static Users()
		{
			Store = new DiskStore("data/users.db", data =>
			{
				var user = User.Deserialize(data);
				Db[user.Login] = user;
			});
		}

		public static async Task<User> GetOrAdd(string login, Func<User> create)
		{
			var added = false;
			var user = Db.GetOrAdd(login, l =>
			{
				added = true;
				return create();
			});
			if(added)
				await Store.WriteAsync(user.Serialize());
			return user;
		}

		public static User Find(string login)
		{
			return Db.GetOrDefault(login);
		}

		private static readonly ConcurrentDictionary<string, User> Db = new ConcurrentDictionary<string, User>(StringComparer.Ordinal);
		private static readonly DiskStore Store;
	}

	internal class User
	{
		public byte[] Serialize()
		{
			return new BinPack().Write(Login).Write(Pass).Write(Allergens).ToArray();
		}

		public static User Deserialize(byte[] buffer)
		{
			var unpack = new BinUnpack(buffer);
			return new User
			{
				Login = unpack.ReadString(),
				Pass = unpack.ReadString(),
				Allergens = unpack.ReadStringArray()
			};
		}

		public string Login;
		public string Pass;
		public string[] Allergens;
	}
}