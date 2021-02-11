using System;
using Autofac;
using MyNoSqlServer.Abstractions;
using Service.ClientWallets.MyNoSql;

namespace Service.ClientWallets.Modules
{
    public class MyNoSqlModule : Module
    {
        private readonly Func<string> _myNoSqlServerWriterUrl;

        public MyNoSqlModule(Func<string> myNoSqlServerWriterUrl)
        {
            _myNoSqlServerWriterUrl = myNoSqlServerWriterUrl;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterMyNoSqlWriter<ClientWalletNoSqlEntity>(builder, ClientWalletNoSqlEntity.TableName);
        }

        private void RegisterMyNoSqlWriter<TEntity>(ContainerBuilder builder, string table)
            where TEntity : IMyNoSqlDbEntity, new()
        {
            builder.Register(ctx => new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<TEntity>(_myNoSqlServerWriterUrl, table, true))
                .As<IMyNoSqlServerDataWriter<TEntity>>()
                .SingleInstance();

        }
    }
}