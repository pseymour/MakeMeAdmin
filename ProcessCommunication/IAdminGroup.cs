// <copyright file="IAdminGroup.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ServiceModel;

    [ServiceContract(Namespace = "http://apps.sinclair.edu/makemeadmin/2016/04/")]
    public interface IAdminGroup
    {
        [OperationContract]
        void AddPrincipalToAdministratorsGroup(string principalSid, DateTime expirationTime);

        [OperationContract]
        void RemovePrincipalFromAdministratorsGroup(string principalSid, RemovalReason reason);

        [OperationContract]
        bool PrincipalIsInList(string principalSid);
    }
}
