﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    [EntityFrameworkInternal]
    public partial class VBUserInstanceEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.VBUserInstance",
                typeof(VBUserInstance),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(VBUserInstance)),
                propertyCount: 25,
                navigationCount: 1,
                servicePropertyCount: 1,
                foreignKeyCount: 1,
                namedIndexCount: 1,
                keyCount: 1);

            var vBUserInstanceID = runtimeEntityType.AddProperty(
                "VBUserInstanceID",
                typeof(Guid),
                propertyInfo: typeof(VBUserInstance).GetProperty("VBUserInstanceID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_VBUserInstanceID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            vBUserInstanceID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var hostname = runtimeEntityType.AddProperty(
                "Hostname",
                typeof(string),
                propertyInfo: typeof(VBUserInstance).GetProperty("Hostname", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_Hostname", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 100);
            hostname.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(VBUserInstance).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(VBUserInstance).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isUserDefined = runtimeEntityType.AddProperty(
                "IsUserDefined",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("IsUserDefined", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_IsUserDefined", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isUserDefined.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var loginDate = runtimeEntityType.AddProperty(
                "LoginDate",
                typeof(DateTime?),
                propertyInfo: typeof(VBUserInstance).GetProperty("LoginDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_LoginDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            loginDate.AddAnnotation("Relational:ColumnType", "datetime");
            loginDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var logoutDate = runtimeEntityType.AddProperty(
                "LogoutDate",
                typeof(DateTime?),
                propertyInfo: typeof(VBUserInstance).GetProperty("LogoutDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_LogoutDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            logoutDate.AddAnnotation("Relational:ColumnType", "datetime");
            logoutDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var nameResolutionOn = runtimeEntityType.AddProperty(
                "NameResolutionOn",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("NameResolutionOn", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_NameResolutionOn", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            nameResolutionOn.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var serverIPV4 = runtimeEntityType.AddProperty(
                "ServerIPV4",
                typeof(string),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServerIPV4", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServerIPV4", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 15,
                unicode: false);
            serverIPV4.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var serverIPV6 = runtimeEntityType.AddProperty(
                "ServerIPV6",
                typeof(string),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServerIPV6", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServerIPV6", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 50,
                unicode: false);
            serverIPV6.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var serviceAppEnabledTCP = runtimeEntityType.AddProperty(
                "ServiceAppEnabledTCP",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServiceAppEnabledTCP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServiceAppEnabledTCP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            serviceAppEnabledTCP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var serviceAppEnbledHTTP = runtimeEntityType.AddProperty(
                "ServiceAppEnbledHTTP",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServiceAppEnbledHTTP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServiceAppEnbledHTTP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            serviceAppEnbledHTTP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var serviceObserverEnabledTCP = runtimeEntityType.AddProperty(
                "ServiceObserverEnabledTCP",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServiceObserverEnabledTCP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServiceObserverEnabledTCP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            serviceObserverEnabledTCP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var servicePortHTTP = runtimeEntityType.AddProperty(
                "ServicePortHTTP",
                typeof(int),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServicePortHTTP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServicePortHTTP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            servicePortHTTP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var servicePortObserverHTTP = runtimeEntityType.AddProperty(
                "ServicePortObserverHTTP",
                typeof(int),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServicePortObserverHTTP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServicePortObserverHTTP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            servicePortObserverHTTP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var servicePortTCP = runtimeEntityType.AddProperty(
                "ServicePortTCP",
                typeof(int),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServicePortTCP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServicePortTCP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            servicePortTCP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var serviceWorkflowEnabledHTTP = runtimeEntityType.AddProperty(
                "ServiceWorkflowEnabledHTTP",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServiceWorkflowEnabledHTTP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServiceWorkflowEnabledHTTP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            serviceWorkflowEnabledHTTP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var serviceWorkflowEnabledTCP = runtimeEntityType.AddProperty(
                "ServiceWorkflowEnabledTCP",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("ServiceWorkflowEnabledTCP", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_ServiceWorkflowEnabledTCP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            serviceWorkflowEnabledTCP.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sessionCount = runtimeEntityType.AddProperty(
                "SessionCount",
                typeof(int),
                propertyInfo: typeof(VBUserInstance).GetProperty("SessionCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_SessionCount", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            sessionCount.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sessionInfo = runtimeEntityType.AddProperty(
                "SessionInfo",
                typeof(string),
                propertyInfo: typeof(VBUserInstance).GetProperty("SessionInfo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_SessionInfo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            sessionInfo.AddAnnotation("Relational:ColumnType", "text");
            sessionInfo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(VBUserInstance).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(VBUserInstance).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var useIPV6 = runtimeEntityType.AddProperty(
                "UseIPV6",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("UseIPV6", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_UseIPV6", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            useIPV6.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var useTextEncoding = runtimeEntityType.AddProperty(
                "UseTextEncoding",
                typeof(bool),
                propertyInfo: typeof(VBUserInstance).GetProperty("UseTextEncoding", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_UseTextEncoding", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            useTextEncoding.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBUserID = runtimeEntityType.AddProperty(
                "VBUserID",
                typeof(Guid),
                propertyInfo: typeof(VBUserInstance).GetProperty("VBUserID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_VBUserID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            vBUserID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(VBUserInstance).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { vBUserInstanceID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCI_FK_VBUserInstance_VBUserID = runtimeEntityType.AddIndex(
                new[] { vBUserID },
                name: "NCI_FK_VBUserInstance_VBUserID");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBUserID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("VBUserID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var vBUser = declaringEntityType.AddNavigation("VBUser",
                runtimeForeignKey,
                onDependent: true,
                typeof(VBUser),
                propertyInfo: typeof(VBUserInstance).GetProperty("VBUser", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUserInstance).GetField("_VBUser", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var vBUserInstance_VBUser = principalEntityType.AddNavigation("VBUserInstance_VBUser",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<VBUserInstance>),
                propertyInfo: typeof(VBUser).GetProperty("VBUserInstance_VBUser", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBUser).GetField("_VBUserInstance_VBUser", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_VBUserInstance_VBUserID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "VBUserInstance");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
