﻿using System.Collections.Generic;
using System.Linq;
using SimpleWebEditorApplication.Core.Interfaces;
using SimpleWebEditorApplication.Core.Models;

namespace SimpleWebEditorApplication.Core.Repositories
{
    public class AccountSqlRepository : SqlRepositoryBase<string, Account>, IAccountRepository
    {
        public AccountSqlRepository(CoreDbContext context) : base (context)
        {
        }

        public override bool Add(Account item)
        {
            if (item == null)
            {
                return false;
            }
            if (_context.Accounts.Select(acc => acc.UserName).Contains(item.UserName))
            {
                return false;
            }
            _context.Accounts.Add(item);

            // create pages
            var workPage = new Page(item, false);
            _context.Pages.Add(workPage);
            var publishedPage = new Page(item, true);
            _context.Pages.Add(publishedPage);

            _context.SaveChanges();
            return true;
        }

        public override Account Get(string itemId)
        {
            return _context.Accounts.FirstOrDefault(acc => acc.UserName.Equals(itemId));
        }

        public override IEnumerable<Account> GetAll()
        {
            return new List<Account>(_context.Accounts);
        }

        public override bool Remove(Account item)
        {
            if (item == null)
            {
                return false;
            }

            // delete pages
            var workPage = _context.Pages.FirstOrDefault(p => p.Owner.UserName.Equals(item.UserName) && !p.IsPublished);
            workPage?.DeleteFile();
            var publishedPage = _context.Pages.FirstOrDefault(p => p.Owner.UserName.Equals(item.UserName) && p.IsPublished);
            publishedPage?.DeleteFile();

            _context.Accounts.Remove(item);
            _context.SaveChanges();
            return true;
        }
        
    }
}
