using Greatbone.Core;

namespace Ministry.Dietary
{
    ///
    /// The order multiplex directory under shop.
    ///
    public class OrderVariableDir : WebDir, IVariable
    {
        public OrderVariableDir(WebDirContext ctx) : base(ctx)
        {
        }

        ///
        /// Get order's detail.
        ///
        public void @default(WebContext wc)
        {
            string shopid = wc.Var;
            int id = wc.VarSub;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(sql.ToString(), p => p.Put(id).Put(shopid)))
                {
                    var order = dc.ToData<Order>();
                    wc.SendHtmlMajor(200, "", main =>
                    {

                    });
                }
                else
                    wc.SendHtmlMajor(200, "没有记录", main => { });
            }
        }

        public void cannel(WebContext wc)
        {
            string shopid = wc.Var;
            int orderid = wc.VarSub;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(sql.ToString(), p => p.Put(orderid).Put(shopid)))
                {
                    var order = dc.ToData<Order>();
                    wc.SendHtmlMajor(200, "", main =>
                    {

                    });
                }
                else
                    wc.SendHtmlMajor(200, "没有记录", main => { });
            }
        }

        public void pend(WebContext wc)
        {
        }

        public void close(WebContext wc)
        {
        }
    }
}