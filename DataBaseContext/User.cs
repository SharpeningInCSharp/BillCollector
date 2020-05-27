using DataBaseContext.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DataBaseContext
{
	public partial class User : Entity
	{
		public delegate void UserAnouncerHandler(string message);

		private readonly UserAnouncerHandler AnouncerHandler;

		public string Login { get; }

		internal string Pass { get; private set; }

		public List<Expence> Expences { get; private set; }

		private User(string login, string pass) : base(EntityType.User)
		{
			Login = login;
			Pass = pass;
		}

		internal User(UserEntity userEntity) : this(userEntity.Login, userEntity.Password)
		{
			if (userEntity.Expences is null)
				Expences = new List<Expence>();
			else
				Expences = userEntity.Expences.Select(x => new Expence(x)).ToList();
		}

		public User(string login, string pass, UserAnouncerHandler anouncerHandler) : this(login, pass)
		{
			AnouncerHandler = anouncerHandler;

			OutMessage($"You're welcome, {Login}!");
		}

		public void AddExpence(Expence expence)
		{
			if (expence.ItemsCount != 0)
			{
				Expences.Add(expence);
				DataBase.AddExpenceToAsync(Login, expence);
			}
		}

		public bool IsPassValid(string pass)
		{
			return Pass.Equals(pass);
		}

		private void OutMessage(string message)
		{
			AnouncerHandler?.Invoke(message);
		}

		public void ChangePass(string newPass)
		{
			Pass = newPass;
			OutMessage("Your password's been changed!");
		}
	}

	public partial class User
	{

	}
}
