using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class UserInfoDao
    {
        private readonly DBEntities context;

        public UserInfoDao(DBEntities context)
        {
            this.context = context;
        }

        public List<UserInfo> GetListUser()
        {
            return context.UserInfo.ToList();
        }

        public JqueryDatatableParam GetListUser(JqueryDatatableParam param)
        {
            var result = new JqueryDatatableParam();
            var users = context.UserInfo;
            result.UserInfoList = users.OrderBy(s => s.Username).Skip(param.IDisplayStart).Take(param.IDisplayLength).ToList();
            result.TotalRecords = users.Count();
            return result;
        }

        public UserInfo GetUser(string id)
        {
            return context.UserInfo.Find(id);
        }

        public UserInfoModel GetUser(string username, string password)
        {
            var query = from userInfo in context.UserInfo

                        join groupUser in context.GroupUser on userInfo.Id equals groupUser.UserId into groupUserJoin
                        from groupUserLResult in groupUserJoin.DefaultIfEmpty()

                        join _group in context.Group on groupUserLResult.GroupId equals _group.Id into groupJoin
                        from groupLResult in groupJoin.DefaultIfEmpty()

                        join groupRole in context.GroupRole on groupLResult.Id equals groupRole.GroupId into groupRoleJoin
                        from groupRoleLResult in groupRoleJoin.DefaultIfEmpty()

                        join role in context.Role on groupRoleLResult.RoleId equals role.Id into roleJoin
                        from roleLResult in roleJoin.DefaultIfEmpty()

                        where userInfo.Username == username && userInfo.Password == password

                        select new UserInfoModel
                        {
                            Id = userInfo.Id,
                            Username = userInfo.Username,
                            Firstname = userInfo.Firstname,
                            Lastname = userInfo.Lastname,
                            Email = userInfo.Email,
                            Phone = userInfo.Phone,
                            IsSysAdmin = roleLResult.IsSysAdmin,
                        };

            return query.FirstOrDefault();

            // 参考
            /*var sql = @"select 
	                        a.[Id],                                
	                        a.[Username],
	                        a.[Firstname],
	                        a.[Lastname],
	                        a.[Email],
	                        a.[Phone],
	                        b.[Name],
	                        c.[IsSysAdmin]
                        from UserInfo as a
                        left join (select  
				                        rel.[GroupId],
				                        rel.[UserId],
				                        item.[Name] 
			                        from GroupUser as rel
			                        inner join [Group] as item
			                            on rel.GroupId = item.Id) as b
                            on a.Id = b.[UserId]
                        left join (select 
				                        rel.[GroupId],
				                        rel.[RoleId],
				                        item.[IsSysAdmin] 
			                        from GroupRole as rel
			                        inner join [Role] as item
			                            on rel.[RoleId] = item.Id) as c
                            on b.GroupId = c.[GroupId]
                        where 1 = 1
                            and a.[Username] = @username
                            and a.[Password] = @password
                        ";
            object[] param = new object[] { new SqlParameter("@username", username), new SqlParameter("@password", password) };
            var datas = context.Database.SqlQuery<UserInfoModel>(sql, param); */

        }

        public void AddUserInfo(UserInfo entity)
        {
            //ユーザーの保存
            context.UserInfo.Add(entity);
            context.SaveChanges();
        }

        public void UpdateUserInfo(UserInfo entity)
        {
            context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        public string DeleteUserInfo(string id)
        {
            var entity = context.UserInfo.Find(id);
            if (entity != null)
            {
                context.UserInfo.Remove(entity);
                context.SaveChanges();
            }

            return string.Empty;
        }

        public List<AuthorInfoModel> GetAuthorInfo(UserInfoModel model)
        {
            IQueryable<AuthorInfoModel> query = null;

            if (model.IsSysAdmin.HasValue && (bool)model.IsSysAdmin)
            {
                query = from userInfo in context.UserInfo

                        join groupUser in context.GroupUser on userInfo.Id equals groupUser.UserId into groupUserJoin
                        from groupUserLResult in groupUserJoin.DefaultIfEmpty()

                        join _group in context.Group on groupUserLResult.GroupId equals _group.Id into groupJoin
                        from groupLResult in groupJoin.DefaultIfEmpty()

                        join groupRole in context.GroupRole on groupLResult.Id equals groupRole.GroupId into groupRoleJoin
                        from groupRoleLResult in groupRoleJoin.DefaultIfEmpty()

                        join role in context.Role on groupRoleLResult.RoleId equals role.Id into roleJoin
                        from roleLResult in roleJoin.DefaultIfEmpty()

                        from resource in context.Resource

                        where userInfo.Id == model.Id

                        select new AuthorInfoModel
                        {
                            Username = userInfo.Username,
                            ResourceId = resource.Id,
                            AppName = resource.AppName,
                            ControllerName = resource.ControllerName,
                            ActionName = resource.ActionName,
                            ResourceName = resource.ResourceName,
                            Type = resource.Type,
                            Level = resource.Level,
                            ParentId = resource.ParentId,
                            Sort = resource.Sort,
                            IconClass = resource.IconClass,
                        };
            }
            else
            {
                query = from userInfo in context.UserInfo

                        join groupUser in context.GroupUser on userInfo.Id equals groupUser.UserId into groupUserJoin
                        from groupUserLResult in groupUserJoin.DefaultIfEmpty()

                        join _group in context.Group on groupUserLResult.GroupId equals _group.Id into groupJoin
                        from groupLResult in groupJoin.DefaultIfEmpty()

                        join groupRole in context.GroupRole on groupLResult.Id equals groupRole.GroupId into groupRoleJoin
                        from groupRoleLResult in groupRoleJoin.DefaultIfEmpty()

                        join role in context.Role on groupRoleLResult.RoleId equals role.Id into roleJoin
                        from roleLResult in roleJoin.DefaultIfEmpty()

                        join roleResource in context.RoleResource on roleLResult.Id equals roleResource.RoleId into roleResourceJoin
                        from roleResourceLResult in roleResourceJoin.DefaultIfEmpty()

                        join resource in context.Resource on roleResourceLResult.ResourceId equals resource.Id into resourceJoin
                        from resourceLResult in resourceJoin.DefaultIfEmpty()

                        where userInfo.Id == model.Id

                        select new AuthorInfoModel
                        {
                            Username = userInfo.Username,
                            ResourceId = resourceLResult.Id,
                            AppName = resourceLResult.AppName,
                            ControllerName = resourceLResult.ControllerName,
                            ActionName = resourceLResult.ActionName,
                            ResourceName = resourceLResult.ResourceName,
                            Type = resourceLResult.Type,
                            Level = resourceLResult.Level,
                            ParentId = resourceLResult.ParentId,
                            Sort = resourceLResult.Sort,
                            IconClass = resourceLResult.IconClass,
                        };
            }

            return GroupAuthorInfo(query.ToList());

            // 参考
            /*var sql = @"select 
	                    a.[Username],
	                    b.[Name],
	                    c.[IsSysAdmin],
                        d.[ResourceId],
	                    d.[AppName],
	                    d.[ControllerName],
	                    d.[ActionName],
                        d.[ResourceName],
	                    d.[Type],
	                    d.[Level],
                        d.[ParentId],
	                    d.[Sort],
	                    d.[IconClass]
                    from UserInfo as a
                    left join (select  
				                    rel.[GroupId],
				                    rel.[UserId],
				                    item.[Name] 
			                    from GroupUser as rel
			                    inner join [Group] as item
			                        on rel.GroupId = item.Id) as b
                        on a.id = b.[UserId]
                    left join (select 
				                    rel.[GroupId],
				                    rel.[RoleId],
				                    item.[IsSysAdmin] 
			                    from GroupRole as rel
			                    inner join Role as item
			                        on rel.[RoleId] = item.Id) as c
                        on b.[GroupId] = c.[GroupId]
                    {0}
                    where a.[Id] = @Id
                    order by
                        d.[Level], 
                        d.[Sort]
                ";

            if (model.IsSysAdmin)
            {
                sql = string.Format(sql, @"left join (select  
				                                            item.[Id] as 'ResourceId',
				                                            item.[AppName],
				                                            item.[ControllerName],
				                                            item.[ActionName],
                                                            item.[ResourceName],
				                                            item.[Type],
				                                            item.[Level],
                                                            item.[ParentId],
				                                            item.[Sort],
				                                            item.[IconClass]
			                                            from Resource as item) as d
                                                on 1 = 1");
            }
            else
            {
                sql = string.Format(sql, @"left join (select  
				                                            rel.[RoleId],
				                                            rel.[ResourceId],
				                                            item.[AppName],
				                                            item.[ControllerName],
				                                            item.[ActionName],
                                                            item.[ResourceName],
				                                            item.[Type],
				                                            item.[Level],
                                                            item.[ParentId],
				                                            item.[Sort],
				                                            item.[IconClass]
			                                            from RoleResource as rel
			                                            inner join Resource as item
			                                                on rel.[ResourceId] = item.Id) as d
                                                on c.[RoleId] = d.[RoleId]");
            }

            object[] param = new object[] { new SqlParameter("@Id", model.Id) };
            var datas = context.Database.SqlQuery<AuthorInfoModel>(sql, param); */
        }


        private List<AuthorInfoModel> GroupAuthorInfo(List<AuthorInfoModel> list)
        {
            if (list != null && list.Any())
            {
                list = list.OrderBy(s => s.Level).ThenBy(s => s.Sort).ToList();

                List<AuthorInfoModel> groupList = new List<AuthorInfoModel>();

                foreach (var obj in list)
                {
                    var parentItem = groupList.Where(s => s.ResourceId == obj.ParentId).FirstOrDefault();
                    if (parentItem != null)
                    {
                        if (parentItem.ChildList == null)
                        {
                            parentItem.ChildList = new List<AuthorInfoModel>();
                        }

                        parentItem.ChildList.Add(obj);
                    }
                    else
                    {
                        groupList.Add(obj);
                    }
                }

                return groupList;
            }

            return list;
        }

        public UserInfo GetFirstAdminUser()
        {
            var query = from role in context.Role
                        join groupRole in context.GroupRole on role.Id equals groupRole.RoleId
                        join groupUser in context.GroupUser on groupRole.GroupId equals groupUser.GroupId
                        join userInfo in context.UserInfo on groupUser.UserId equals userInfo.Id
                        where role.IsSysAdmin == true
                        select userInfo;
            return query.FirstOrDefault();
        }
    }
}