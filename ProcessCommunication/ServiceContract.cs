// <copyright file="ServiceContract.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.ServiceModel;

    [ServiceContract(Namespace = "http://apps.sinclair.edu")]
    public interface IServiceContract
    {
        [OperationContract]
        void AddPrincipalToAdministratorsGroup(ContextType contextType, string contextName, string sidToAdd);

        [OperationContract]
        void RemoveUserFromAdministratorsGroup(string sidToRemove);
    }    
}
