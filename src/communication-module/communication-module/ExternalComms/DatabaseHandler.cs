using Core.ImageReceiver;
using custom_message_based_implementation.model;
using custom_message_based_implementation.proxy;
using message_based_communication.connection;

namespace Core.ExternalComms
{
    internal class DatabaseHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private DatabaseServermoduleProxy DatabaseServermoduleProxy { get; }

        private PrimaryKey _primaryKey;

        internal DatabaseHandler(ProxyHelper ph, ClientModuleCommunication cmm)
        {
            this.DatabaseServermoduleProxy = new DatabaseServermoduleProxy(ph, cmm);
        }

        internal string Login(EmailAndPasswordWrapper parameters)
        {
            _primaryKey = null;
            Logger.Info("Login initiated, " + nameof(_primaryKey) + " set to null");
            DatabaseServermoduleProxy.Login(new Email{TheEmail = parameters.Email }, new Password{ThePassword = parameters.Password}, LoginCallback);
            GeneralHandler.PollVariableFor10Seconds(ref _primaryKey);
            return _primaryKey.TheKey.ToString();
        }

        internal string CreateAccount(EmailAndPasswordWrapper parameters)
        {
            _primaryKey = null;
            Logger.Info("CreateAccount initiated, " + nameof(_primaryKey) + " set to null");
            DatabaseServermoduleProxy.CreateAccount(new Email { TheEmail = parameters.Email }, new Password { ThePassword = parameters.Password }, CreateAccountCallback);
            GeneralHandler.PollVariableFor10Seconds(ref _primaryKey);
            return _primaryKey.TheKey.ToString();
        }

        // Callbacks
        private void LoginCallback(PrimaryKey primaryKey)
        {
            _primaryKey = primaryKey;
            Logger.Info(this._primaryKey.GetType().Name + " " + nameof(this._primaryKey) + " set");
        }

        private void CreateAccountCallback(PrimaryKey primaryKey)
        {
            _primaryKey = primaryKey;
            Logger.Info(this._primaryKey.GetType().Name + " " + nameof(this._primaryKey) + " set");
        }
    }
}