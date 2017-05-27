﻿namespace Fixie.Execution
{
    using System;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;

    public class ExecutionEnvironment : IDisposable
    {
        readonly string assemblyFullPath;
        readonly AppDomain appDomain;
        readonly string previousWorkingDirectory;
        readonly RemoteAssemblyResolver assemblyResolver;
        readonly ExecutionProxy executionProxy;

        public ExecutionEnvironment(string assemblyPath)
        {
            assemblyFullPath = Path.GetFullPath(assemblyPath);
            appDomain = CreateAppDomain(assemblyFullPath);

            previousWorkingDirectory = Directory.GetCurrentDirectory();
            var assemblyDirectory = Path.GetDirectoryName(assemblyFullPath);
            Directory.SetCurrentDirectory(assemblyDirectory);

            assemblyResolver = CreateFrom<RemoteAssemblyResolver>();
            assemblyResolver.RegisterAssemblyLocation(typeof(ExecutionProxy).Assembly.Location);

            executionProxy = Create<ExecutionProxy>(previousWorkingDirectory);
        }

        public void Subscribe<TListener>(params object[] listenerArguments) where TListener : Listener
        {
            assemblyResolver.RegisterAssemblyLocation(typeof(TListener).Assembly.Location);
            executionProxy.Subscribe<TListener>(listenerArguments);
        }

        public void DiscoverMethods(string[] runnerArguments, string[] conventionArguments)
            => executionProxy.DiscoverMethods(assemblyFullPath, runnerArguments, conventionArguments);

        public int RunAssembly(string[] runnerArguments, string[] conventionArguments)
            => executionProxy.RunAssembly(assemblyFullPath, runnerArguments, conventionArguments);

        public void RunMethods(string[] runnerArguments, string[] conventionArguments, string[] methods)
            => executionProxy.RunMethods(assemblyFullPath, runnerArguments, conventionArguments, methods);

        T CreateFrom<T>() where T : LongLivedMarshalByRefObject, new()
        {
            return (T)appDomain.CreateInstanceFromAndUnwrap(typeof(T).Assembly.Location, typeof(T).FullName, false, 0, null, null, null, null);
        }

        T Create<T>(params object[] arguments) where T : LongLivedMarshalByRefObject
        {
            return (T)appDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName, false, 0, null, arguments, null, null);
        }

        public void Dispose()
        {
            executionProxy.Dispose();
            assemblyResolver.Dispose();
            AppDomain.Unload(appDomain);
            Directory.SetCurrentDirectory(previousWorkingDirectory);
        }

        static AppDomain CreateAppDomain(string assemblyFullPath)
        {
            var setup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(assemblyFullPath),
                ApplicationName = Guid.NewGuid().ToString(),
                ConfigurationFile = GetOptionalConfigFullPath(assemblyFullPath)
            };

            return AppDomain.CreateDomain(setup.ApplicationName, null, setup, new PermissionSet(PermissionState.Unrestricted));
        }

        static string GetOptionalConfigFullPath(string assemblyFullPath)
        {
            var configFullPath = assemblyFullPath + ".config";

            return File.Exists(configFullPath) ? configFullPath : null;
        }
    }
}