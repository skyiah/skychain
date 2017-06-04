﻿using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public abstract class ChargeWork<V> : Work where V : ChargeVarWork
    {
        protected ChargeWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, int>(obj => ((Charge) obj).id);
        }
    }

    [Ui("举报")]
    [User]
    public class MyChargeWork : ChargeWork<MyChargeVarWork>
    {
        public MyChargeWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM tipoffs WHERE wx = @1 ORDER BY id DESC", p => p.Set(wx)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Charge>(-1));
                }
                else
                {
                    ac.GiveGridPage(200, (Charge[]) null);
                }
            }
        }
    }

    [Ui("举报管理")]
    [User(spr: true)]
    public class SprChargeWork : ChargeWork<SprChargeVarWork>
    {
        public SprChargeWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ Item.ICON;
                dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1");
                if (dc.Query(p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Item>(proj), proj ^ Item.SHOPID);
                }
                else
                {
                    ac.GiveGridPage(200, (Item[]) null);
                }
            }
        }
    }
}