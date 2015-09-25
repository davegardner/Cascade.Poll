using Cascade.Poll.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;

namespace Cascade.Poll {
    public class Migrations : DataMigrationImpl {

        public int Create() {
			// Creating table PollHolderRecord
			SchemaBuilder.CreateTable("PollHolderRecord", table => table
				.ContentPartRecord()
				.Column<int>("PollId")
			);

			// Creating table PollRecord
			SchemaBuilder.CreateTable("PollRecord", table => table
				.Column<int>("Id", column => column.PrimaryKey().Identity())
				.Column<string>("Question")
				.Column<DateTime>("StartDate",column => column.Nullable())
                .Column<DateTime>("EndDate", column => column.Nullable())
				.Column<string>("PollState")
				.Column<int>("MaxAnswers")
			);

			// Creating table PollLogRecord
			SchemaBuilder.CreateTable("PollLogRecord", table => table
				.Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<DateTime>("VoteDate")
                .Column<string>("UserDetail")
				.Column<int>("PollRecord_id")
			);

			// Creating table PollAnswerRecord
			SchemaBuilder.CreateTable("PollAnswerRecord", table => table
				.Column<int>("Id", column => column.PrimaryKey().Identity())
				.Column<string>("Answer")
				.Column<int>("Votes")
				.Column<int>("PollRecord_id")
			);


            return 1;
        }
        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterPartDefinition(
                typeof(PollPart).Name, cfg => cfg.Attachable());
            
            // Create a new widget content type with our poll
            ContentDefinitionManager.AlterTypeDefinition("PollWidget", cfg => cfg
                .WithPart(typeof(PollPart).Name)
                .WithPart("CommonPart")
                .WithPart("WidgetPart")
                .WithSetting("Stereotype", "Widget"));

            return 2;
        }
    }
}