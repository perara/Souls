using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsModel.Migrations.Clean
{

    [Migration(1)]
    public class CreateUsers : Migration
    {

        public override void Up()
        {
            Insert.IntoTable("Ability").Row(new
            {
                name = "Taunt",
                parameter = "null"
            });

            Insert.IntoTable("Ability").Row(new
            {
                name = "Heal",
                parameter = "null"
            });

            Insert.IntoTable("Ability").Row(new
            {
                name = "Fist",
                parameter = ""
            });

            Insert.IntoTable("Ability").Row(new
            {
                name = "Something",
                parameter = ""
            });

            Insert.IntoTable("Ability").Row(new
            {
                name = "Break Bones",
                parameter = ""
            });
        }

        public override void Down()
        {
            Delete.FromTable("Ability").AllRows();
        }


    }
}
