
using Corno.Data.Context;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.Common;
using Corno.Services.Common.Interfaces;
using Corno.Services.SMS;
using Corno.Services.SMS.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Corno.Data.Login;
using Corno.Services.Email;
using Corno.Services.Email.Interfaces;
using Corno.Services.Encryption;
using Corno.Services.Encryption.Interfaces;
using Corno.Services.Http;
using Corno.Services.Http.Interfaces;
using Telerik.Reporting;
using Telerik.WinControls.UI;
using Unity.Mvc5;

namespace Corno.Services.Bootstrapper
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        public static IUnityContainer Initialise(BaseContext dbContext)
        {
            var container = BuildUnityContainer(dbContext);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        public static IUnityContainer SetResolver(IUnityContainer container)
        {
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer(BaseContext dbContext = null)
        {
            var container = new UnityContainer();

            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager(),
                null == dbContext
                    ? new InjectionConstructor("Name = " + FieldConstants.CornoContext)
                    : new InjectionConstructor(dbContext));


            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType(typeof(IdentityManager));

            // Masters 

            container.RegisterType(typeof(ICompanyService), typeof(CompanyService));
            
            container.RegisterType<ICompanyService, CompanyService>();
            container.RegisterType<IStateService, StateService>();
            container.RegisterType<ICityService, CityService>();
            

            // SMS
            container.RegisterType<IBhashSmsService, BhashSmsService>();
            container.RegisterType<ISmsSettingsService, SmsSettingsService>();

            // Http
            container.RegisterType<IHttpService, HttpService>();

            // Email
            container.RegisterType<IEmailService, EmailService>();

            // Encryption
            container.RegisterType<IAes256Service, Aes256Service>();
            container.RegisterType<IJWTService, JWTService>();

            return container;
        }

        public static RadForm GetForm(Type type)
        {
            if (null == GlobalVariables.Container)
                GlobalVariables.Container = BuildUnityContainer();

            return (RadForm) GlobalVariables.Container.Resolve(type);
        }

        public static Report GetReport(Type type)
        {
            if (null == GlobalVariables.Container)
                GlobalVariables.Container = BuildUnityContainer();

            return (Report) GlobalVariables.Container.Resolve(type);
        }

        //public static IService GetService(Type type)
        //{
        //    if (null == GlobalVariables.Container)
        //        GlobalVariables.Container = BuildUnityContainer();

        //    return (IService)GlobalVariables.Container.Resolve(type);
        //}

        public static object GetService(Type serviceType)
        {
            if (null == GlobalVariables.Container)
                GlobalVariables.Container = BuildUnityContainer();

            if ((serviceType.IsClass && !serviceType.IsAbstract) || GlobalVariables.Container.IsRegistered(serviceType))
                return GlobalVariables.Container.Resolve(serviceType);
            return null;
        }

        public static void RegisterType(Type interfaceType, Type serviceType)
        {
            if (null == GlobalVariables.Container)
                GlobalVariables.Container = BuildUnityContainer();

            GlobalVariables.Container.RegisterType(interfaceType, serviceType);
        }

        public static void RegisterType(Type interfaceType, Type serviceType, LifetimeManager lifetimeManager)
        {
            if (null == GlobalVariables.Container)
                GlobalVariables.Container = BuildUnityContainer();

            GlobalVariables.Container.RegisterType(interfaceType, serviceType, lifetimeManager);
        }

        public static IEnumerable<object> GetServices(Type serviceType)
        {
            if (null == GlobalVariables.Container)
                GlobalVariables.Container = BuildUnityContainer();

            if ((serviceType.IsClass && !serviceType.IsAbstract) || GlobalVariables.Container.IsRegistered(serviceType))
                return GlobalVariables.Container.ResolveAll(serviceType);

            return new object[] { };
        }
    }
}