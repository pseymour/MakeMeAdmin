// <copyright file="ServiceContract.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ServiceModel;

    [ServiceContract(Namespace = "http://apps.sinclair.edu")]
    public interface IServiceContract
    {
        [OperationContract]
        void AddPrincipalToAdministratorsGroup(string principalSid, DateTime expirationTime);

        [OperationContract]
        void RemovePrincipalFromAdministratorsGroup(string principalSid);
    }    
}
