﻿using Autofac;
using ClusterManager.Dto;
using ClusterManager.Dto.Infrastructures;
using ClusterManager.Dao.Infrastructures;
using ClusterManager.Dao;
using ClusterManager.Core;
using ClusterManager.Core.Infrastructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.DI
{
    internal sealed class RegisterModel:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IoTHubResourceDto>().As<IIoTHubResourceDto>();
            builder.RegisterType<TokenDto>().As<ITokenDto>();
            builder.RegisterType<ResourceGroupDto>().As<IResourceGroupDto>();
            builder.RegisterType<AKSDto>().As<IAKSDto>();
            builder.RegisterType<MonitorDto>().As<IMonitorDto>();
            builder.RegisterType<SubscriptionDto>().As<ISubscriptionDto>();
            builder.RegisterType<AccountDao>().As<IAccountDao>();
            //builder.RegisterType<TokenDao>().As<ITokenDao>();
            //Bus-IBus
            builder.RegisterType<IoTHubResourceBus>().As<IIoTHubResourceBus>();
            builder.RegisterType<ResourceGroupBus>().As<IResourceGroupBus>();
            builder.RegisterType<AKSBus>().As<IAKSBus>();
            builder.RegisterType<AccountBus>().As<IAccountBus>();
            builder.RegisterType<AuthBus>().As<IAuthBus>();
            builder.RegisterType<MonitorBus>().As<IMonitorBus>();
            //base.Load(builder);
           
        }
    }
}
