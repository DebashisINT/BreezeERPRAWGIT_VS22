﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewCompany.Models.DBML
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="PK14052019")]
	public partial class NewCompanyDBMLDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public NewCompanyDBMLDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["breeze_developmentConnectionString1"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public NewCompanyDBMLDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public NewCompanyDBMLDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public NewCompanyDBMLDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public NewCompanyDBMLDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<V_AssignUser> V_AssignUsers
		{
			get
			{
				return this.GetTable<V_AssignUser>();
			}
		}
		
		public System.Data.Linq.Table<V_TaskCreation> V_TaskCreations
		{
			get
			{
				return this.GetTable<V_TaskCreation>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.V_AssignUser")]
	public partial class V_AssignUser
	{
		
		private System.Nullable<long> _serial;
		
		private System.Nullable<long> _TASK_ID;
		
		private string _UserID;
		
		private string _username;
		
		public V_AssignUser()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_serial", DbType="BigInt")]
		public System.Nullable<long> serial
		{
			get
			{
				return this._serial;
			}
			set
			{
				if ((this._serial != value))
				{
					this._serial = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TASK_ID", DbType="BigInt")]
		public System.Nullable<long> TASK_ID
		{
			get
			{
				return this._TASK_ID;
			}
			set
			{
				if ((this._TASK_ID != value))
				{
					this._TASK_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserID", DbType="VarChar(20)")]
		public string UserID
		{
			get
			{
				return this._UserID;
			}
			set
			{
				if ((this._UserID != value))
				{
					this._UserID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_username", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string username
		{
			get
			{
				return this._username;
			}
			set
			{
				if ((this._username != value))
				{
					this._username = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.V_TaskCreation")]
	public partial class V_TaskCreation
	{
		
		private long _ID;
		
		private string _TASK_SUBJECT;
		
		private string _TASK_DESCRIPTION;
		
		private string _CreatedBy;
		
		private string _ModyfiedBy;
		
		private System.Nullable<System.DateTime> _CREATEDON;
		
		private System.Nullable<System.DateTime> _TASK_STARTDATE;
		
		private System.Nullable<System.DateTime> _TASK_DUEDATE;
		
		private string _INTERVAL;
		
		private System.Nullable<System.DateTime> _ModyfiedOn;
		
		private string _ISACTIVE;
		
		private string _flag;
		
		private string _userlist;
		
		public V_TaskCreation()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="BigInt NOT NULL")]
		public long ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TASK_SUBJECT", DbType="NVarChar(500)")]
		public string TASK_SUBJECT
		{
			get
			{
				return this._TASK_SUBJECT;
			}
			set
			{
				if ((this._TASK_SUBJECT != value))
				{
					this._TASK_SUBJECT = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TASK_DESCRIPTION", DbType="NVarChar(1000)")]
		public string TASK_DESCRIPTION
		{
			get
			{
				return this._TASK_DESCRIPTION;
			}
			set
			{
				if ((this._TASK_DESCRIPTION != value))
				{
					this._TASK_DESCRIPTION = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedBy", DbType="VarChar(50)")]
		public string CreatedBy
		{
			get
			{
				return this._CreatedBy;
			}
			set
			{
				if ((this._CreatedBy != value))
				{
					this._CreatedBy = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ModyfiedBy", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string ModyfiedBy
		{
			get
			{
				return this._ModyfiedBy;
			}
			set
			{
				if ((this._ModyfiedBy != value))
				{
					this._ModyfiedBy = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CREATEDON", DbType="DateTime")]
		public System.Nullable<System.DateTime> CREATEDON
		{
			get
			{
				return this._CREATEDON;
			}
			set
			{
				if ((this._CREATEDON != value))
				{
					this._CREATEDON = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TASK_STARTDATE", DbType="DateTime")]
		public System.Nullable<System.DateTime> TASK_STARTDATE
		{
			get
			{
				return this._TASK_STARTDATE;
			}
			set
			{
				if ((this._TASK_STARTDATE != value))
				{
					this._TASK_STARTDATE = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TASK_DUEDATE", DbType="DateTime")]
		public System.Nullable<System.DateTime> TASK_DUEDATE
		{
			get
			{
				return this._TASK_DUEDATE;
			}
			set
			{
				if ((this._TASK_DUEDATE != value))
				{
					this._TASK_DUEDATE = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_INTERVAL", DbType="VarChar(26)")]
		public string INTERVAL
		{
			get
			{
				return this._INTERVAL;
			}
			set
			{
				if ((this._INTERVAL != value))
				{
					this._INTERVAL = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ModyfiedOn", DbType="DateTime")]
		public System.Nullable<System.DateTime> ModyfiedOn
		{
			get
			{
				return this._ModyfiedOn;
			}
			set
			{
				if ((this._ModyfiedOn != value))
				{
					this._ModyfiedOn = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ISACTIVE", DbType="VarChar(8) NOT NULL", CanBeNull=false)]
		public string ISACTIVE
		{
			get
			{
				return this._ISACTIVE;
			}
			set
			{
				if ((this._ISACTIVE != value))
				{
					this._ISACTIVE = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_flag", DbType="VarChar(1) NOT NULL", CanBeNull=false)]
		public string flag
		{
			get
			{
				return this._flag;
			}
			set
			{
				if ((this._flag != value))
				{
					this._flag = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_userlist", DbType="NVarChar(MAX)")]
		public string userlist
		{
			get
			{
				return this._userlist;
			}
			set
			{
				if ((this._userlist != value))
				{
					this._userlist = value;
				}
			}
		}
	}
}
#pragma warning restore 1591