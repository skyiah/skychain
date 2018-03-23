using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Core
{
    public abstract class OrgWork<V> : Work where V : OrgVarWork
    {
        protected OrgWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Org) obj).id);
        }
    }

    [Ui("网点")]
    public class AdmOrgWork : OrgWork<AdmOrgVarWork>
    {
        public AdmOrgWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs ORDER BY id");
                var arr = dc.Query<Org>();
                ac.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARDVIEW(arr,
                        o => { h.T(o.name).T(" / ").T(o.id); },
                        o =>
                        {
                            h.FIELD(o.descr, "简介");
                            h.FIELD_("地址").T(o.addr)._FIELD();
                            h.FIELD_("坐标").T(o.x)._T(o.y)._FIELD();
                            h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                        });
                });
            }
        }


        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext ac)
        {
            const byte proj = Org.ADM;
            if (ac.GET)
            {
                var o = new Org { };
                o.Read(ac.Query, proj);
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.id), o.id, "编号", max: 4, min: 4, required: true);
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.TEXTAREA(nameof(o.descr), o.descr, "简介", max: 50, required: true);
                    m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度", max: 20, width: 6).NUMBER(nameof(o.x), o.x, "纬度", max: 20, width: 6);
                    m._FORM();
                });
            }

            else // post
            {
                var o = await ac.ReadObjectAsync<Org>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO orgs")._(Org.Empty, proj)._VALUES_(Org.Empty, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.GivePane(200); // created
            }
        }
    }
}