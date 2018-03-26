﻿using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Core.User;

namespace Core
{
    public abstract class CashWork : Work
    {
        protected CashWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [Ui("财务记账"), User(OPRMGR)]
    public class OprCashWork : CashWork
    {
        public OprCashWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac, int page)
        {
            string orgid = ac[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Cash>("SELECT * FROM cashes WHERE orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
                ac.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLEVIEW(arr,
                        () => h.TH("日期").TH("项目").TH("收入").TH("支出").TD("记账"),
                        o => h.TD(o.date).TD(Cash.Codes[o.code]).TD(o.receive).TD(o.pay).TD(o.creator));
                }, false, 2);
            }
        }

        [Ui("记账"), Tool(ButtonShow)]
        public async Task entry(WebContext ac)
        {
            string orgid = ac[-1];
            Cash o = null;
            if (ac.GET)
            {
                o = new Cash() { };
                o.Read(ac.Query);
                ac.GivePane(200, h =>
                {
                    h.FORM_();

                    h.FIELDSET_("填写交易信息");
                    h.SELECT(nameof(o.code), o.code, Cash.Codes, label: "类　型");
                    h.TEXT(nameof(o.descr), o.descr, "简　述", max: 20);
                    h.FIELD_("收／支").NUMBER(nameof(o.receive), o.receive, tip: "收入").NUMBER(nameof(o.pay), o.pay, tip: "支出")._FIELD();
                    h._FIELDSET();

                    h._FORM();
                });
                return;
            }
            o = await ac.ReadObjectAsync(obj: new Cash
            {
                orgid = orgid,
                date = DateTime.Now,
                creator = ((User) ac.Principal).name
            });
            using (var dc = NewDbContext())
            {
                const byte proj = 0xff ^ Cash.ID;
                dc.Sql("INSERT INTO cashes")._(Cash.Empty, proj)._VALUES_(Cash.Empty, proj);
                dc.Execute(p => o.Write(p, proj));
            }
            ac.GivePane(200);
        }

        [Ui("月报"), Tool(ButtonOpen, 2)]
        public void monthly(WebContext ac)
        {
            string orgid = ac[-1];
            ac.GivePane(200, m =>
            {
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT to_char(date, 'YYYY-MM') as yrmon, code, SUM(receive), SUM(pay) FROM cashes WHERE orgid = @1 GROUP BY yrmon, txn ORDER BY yrmon DESC", p => p.Set(orgid));
                    while (dc.Next())
                    {
                        dc.Let(out string yrmon).Let(out short txn).Let(out decimal recieved).Let(out decimal paid);
                        m.FIELDSET_(yrmon);

                        m._FIELDSET();
                    }
                }
            }, false, 3);
        }
    }
}