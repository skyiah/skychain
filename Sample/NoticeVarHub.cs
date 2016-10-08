﻿using Greatbone.Core;

namespace Greatbone.Sample
{
    public class NoticeVarHub : WebVarHub
    {
        public NoticeVarHub(ISetting setg) : base(setg)
        {
        }

        /// <summary>
        /// Gets a particular notice.
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="id"></param>
        public override void @default(WebContext wc, string id)
        {

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT * FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    Notice obj = dc.GetObj<Notice>();
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Deletes the target notice.
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="id"></param>
        public void del(WebContext wc, string id)
        {
            string userid = wc.Token.Key;

            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM notices WHERE id = @1 AND authorid = @2", p => p.Put(id).Put(userid)) > 0)
                {

                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// To join/enlist the current user to the target notice.
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="noticeid"></param>
        public void ask(WebContext wc, string noticeid)
        {
            string userid = wc.Token.Key;

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT joins FROM notices WHERE id = @1", p => p.Put(userid)))
                {
                    // parse to list

                    // update back the table
                    Ask[] arr = null;
                    if (dc.Execute("UPDATE notices SET joins = @1", p => p.Put(arr)) > 0)
                    {

                    }

                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

    }
}