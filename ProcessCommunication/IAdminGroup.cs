// <copyright file="IAdminGroup.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "http://apps.sinclair.edu/makemeadmin/2017/10/")]
    public interface IAdminGroup
    {
        [OperationContract]
        void AddPrincipalToAdministratorsGroup();

        [OperationContract]
        void RemovePrincipalFromAdministratorsGroup(RemovalReason reason);

        [OperationContract]
        bool PrincipalIsInList();
    }

}
