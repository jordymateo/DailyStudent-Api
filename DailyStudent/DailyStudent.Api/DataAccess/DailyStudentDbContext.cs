using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DailyStudent.Api.DataAccess
{
    public partial class DailyStudentDbContext : DbContext
    {
        public DailyStudentDbContext()
        {
        }

        public DailyStudentDbContext(DbContextOptions<DailyStudentDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AcademicPeriod> AcademicPeriods { get; set; }
        public virtual DbSet<AcademicPeriodCourse> AcademicPeriodCourse { get; set; }
        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<AttachedDocument> AttachedDocument { get; set; }
        public virtual DbSet<Career> Career { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<CourseType> CourseType { get; set; }
        public virtual DbSet<EventLog> EventLog { get; set; }
        public virtual DbSet<Institution> Institution { get; set; }
        public virtual DbSet<InstitutionUser> InstitutionUser { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<Pensum> Pensum { get; set; }
        public virtual DbSet<Subject> Subject { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserCareer> UserCareer { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UserLoginDevice> UserLoginDevice { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("btree_gin")
                .HasPostgresExtension("btree_gist")
                .HasPostgresExtension("citext")
                .HasPostgresExtension("cube")
                .HasPostgresExtension("dblink")
                .HasPostgresExtension("dict_int")
                .HasPostgresExtension("dict_xsyn")
                .HasPostgresExtension("earthdistance")
                .HasPostgresExtension("fuzzystrmatch")
                .HasPostgresExtension("hstore")
                .HasPostgresExtension("intarray")
                .HasPostgresExtension("ltree")
                .HasPostgresExtension("pg_stat_statements")
                .HasPostgresExtension("pg_trgm")
                .HasPostgresExtension("pgcrypto")
                .HasPostgresExtension("pgrowlocks")
                .HasPostgresExtension("pgstattuple")
                .HasPostgresExtension("tablefunc")
                .HasPostgresExtension("unaccent")
                .HasPostgresExtension("uuid-ossp")
                .HasPostgresExtension("xml2");

            modelBuilder.Entity<AcademicPeriod>(entity =>
            {
                entity.ToTable("academicperiod");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.EndDate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.InitialDate)
                    .HasColumnName("initialdate")
                    .HasColumnType("date");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.UserCareerId).HasColumnName("usercareerid");

                entity.HasOne(d => d.UserCareer)
                    .WithMany(p => p.AcademicPeriod)
                    .HasForeignKey(d => d.UserCareerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_UserCareerId_academicperiod");
            });

            modelBuilder.Entity<AcademicPeriodCourse>(entity =>
            {
                entity.ToTable("academicperiodcourse");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcademicPeriodId).HasColumnName("academicperiodid");

                entity.Property(e => e.CourseId).HasColumnName("courseid");

                entity.HasOne(d => d.AcademicPeriod)
                    .WithMany(p => p.AcademicPeriodCourse)
                    .HasForeignKey(d => d.AcademicPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AcademicPeriodId_academicperiodcourse");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.AcademicPeriodCourse)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CourseId_academicperiodcourse");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("assignment");

                entity.HasIndex(e => e.Title)
                    .HasName("idx_assignment_title");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CourseId).HasColumnName("courseid");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion");

                entity.Property(e => e.DueDate).HasColumnName("duedate");

                entity.Property(e => e.IsCompleted).HasColumnName("iscompleted");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.IsIndividual).HasColumnName("isindividual");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(150);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CourseId_assignment");
            });

            modelBuilder.Entity<AttachedDocument>(entity =>
            {
                entity.ToTable("attacheddocument");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AssigmentId).HasColumnName("assigmentid");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path")
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Assigment)
                    .WithMany(p => p.AttachedDocument)
                    .HasForeignKey(d => d.AssigmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AssigmentId_attacheddocument");
            });

            modelBuilder.Entity<Career>(entity =>
            {
                entity.ToTable("career");

                entity.HasIndex(e => e.Name)
                    .HasName("idx_career_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprovalDate).HasColumnName("approvaldate");

                entity.Property(e => e.ApproverUserId).HasColumnName("approveruserid");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatorUserId).HasColumnName("creatoruserid");

                entity.Property(e => e.InstitutionId).HasColumnName("institutionid");

                entity.Property(e => e.IsPensumAvailable).HasColumnName("ispensumavailable");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.HasOne(d => d.ApproverUser)
                    .WithMany(p => p.CareerApproveruser)
                    .HasForeignKey(d => d.ApproverUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ApproverUserId_career");

                entity.HasOne(d => d.CreatorUser)
                    .WithMany(p => p.CareerCreatoruser)
                    .HasForeignKey(d => d.CreatorUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CreatorUserId_career");

                entity.HasOne(d => d.Institution)
                    .WithMany(p => p.Career)
                    .HasForeignKey(d => d.InstitutionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_InstitutionId_career");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country");

                entity.HasIndex(e => e.Iso)
                    .HasName("uq_country_iso")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("uq_country_name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Iso)
                    .IsRequired()
                    .HasColumnName("iso")
                    .HasMaxLength(2);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("course");

                entity.HasIndex(e => e.Name)
                    .HasName("idx_course_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasColumnName("color")
                    .HasMaxLength(6);

                entity.Property(e => e.CourseTypeId)
                    .IsRequired()
                    .HasColumnName("coursetypeid")
                    .HasMaxLength(30);

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.InstitutionUserId).HasColumnName("institutionuserid");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.TeacherFullName)
                    .IsRequired()
                    .HasColumnName("teacherfullname")
                    .HasMaxLength(150);

                entity.HasOne(d => d.CourseType)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.CourseTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CourseTypeId_course");

                entity.HasOne(d => d.InstitutionUser)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.InstitutionUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_InstitutionUserId_course");
            });

            modelBuilder.Entity<CourseType>(entity =>
            {
                entity.ToTable("coursetype");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(30);

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatorUserId).HasColumnName("creatoruserid");

                entity.Property(e => e.ModificationDate)
                    .HasColumnName("modificationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.ModifierUserId).HasColumnName("modifieruserid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.ToTable("eventlog");

                entity.HasIndex(e => e.Title)
                    .HasName("idx_eventlog_title");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatorUserId).HasColumnName("creatoruserid");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.Level)
                    .IsRequired()
                    .HasColumnName("level")
                    .HasMaxLength(25);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(255);

                entity.HasOne(d => d.CreatorUser)
                    .WithMany(p => p.EventLog)
                    .HasForeignKey(d => d.CreatorUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_creatoruserid_eventlog");
            });

            modelBuilder.Entity<Institution>(entity =>
            {
                entity.ToTable("institution");

                entity.HasIndex(e => e.Acronym)
                    .HasName("idx_institution_acronym");

                entity.HasIndex(e => e.LogoPath)
                    .HasName("uq_institution_logopath")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("idx_institution_name");

                entity.HasIndex(e => e.Website)
                    .HasName("uq_institution_website")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .IsRequired()
                    .HasColumnName("acronym")
                    .HasMaxLength(25);

                entity.Property(e => e.ApprovalDate).HasColumnName("approvaldate");

                entity.Property(e => e.ApproverUserId).HasColumnName("approveruserid");

                entity.Property(e => e.CountryId).HasColumnName("countryid");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatorUserId).HasColumnName("creatoruserid");

                entity.Property(e => e.IsAvailable).HasColumnName("isavailable");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.LogoPath)
                    .IsRequired()
                    .HasColumnName("logopath")
                    .HasMaxLength(1000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(150);

                entity.Property(e => e.Website)
                    .IsRequired()
                    .HasColumnName("website")
                    .HasMaxLength(1000);

                entity.HasOne(d => d.ApproverUser)
                    .WithMany(p => p.InstitutionApproveruser)
                    .HasForeignKey(d => d.ApproverUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_approveruserid_institution");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Institution)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_countryid_institution");

                entity.HasOne(d => d.CreatorUser)
                    .WithMany(p => p.InstitutionCreatoruser)
                    .HasForeignKey(d => d.CreatorUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_creatoruserid_institution");
            });

            modelBuilder.Entity<InstitutionUser>(entity =>
            {
                entity.ToTable("institutionuser");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.InstitutionId).HasColumnName("institutionid");

                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.HasOne(d => d.Institution)
                    .WithMany(p => p.InstitutionUser)
                    .HasForeignKey(d => d.InstitutionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_institutionid_institutionuser");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.InstitutionUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_userid_institutionuser");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("note");

                entity.HasIndex(e => e.Title)
                    .HasName("idx_note_title");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Courseid).HasColumnName("courseid");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(100);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Note)
                    .HasForeignKey(d => d.Courseid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CourseId_note");
            });

            modelBuilder.Entity<Pensum>(entity =>
            {
                entity.ToTable("pensum");

                entity.HasIndex(e => e.Name)
                    .HasName("idx_pensum_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprovalDate).HasColumnName("approvaldate");

                entity.Property(e => e.ApproverUserId).HasColumnName("approveruserid");

                entity.Property(e => e.CarrerId).HasColumnName("carrerid");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatorUserId).HasColumnName("creatoruserid");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.IsApproved).HasColumnName("isapproved");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.CreditLimitPerPeriod).HasColumnName("creditlimitperperiod");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path")
                    .HasMaxLength(255);


                entity.HasOne(d => d.Career)
                    .WithMany(p => p.Pensums)
                    .HasForeignKey(d => d.CarrerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_careerId_pensum");

                entity.HasOne(d => d.ApproverUser)
                    .WithMany(p => p.PensumApproverUser)
                    .HasForeignKey(d => d.ApproverUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ApproverUserId_pensum");

                entity.HasOne(d => d.CreatorUser)
                    .WithMany(p => p.PensumCreatorUser)
                    .HasForeignKey(d => d.CreatorUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CreatorUserId_pensum");
            });
           

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subject");

                entity.HasIndex(e => e.Name)
                    .HasName("idx_subject_name");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Creationdate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.HasIndex(e => e.Code)
                    .HasName("uq_subjec_code")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(50);

                entity.Property(e => e.Prerequisite)
                    .HasColumnName("prerequisite")
                    .HasMaxLength(100);

                entity.Property(e => e.Corequisite)
                    .HasColumnName("corequisite")
                    .HasMaxLength(100);

                entity.Property(e => e.Credits).HasColumnName("credits");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(150);

                entity.Property(e => e.Pensumid).HasColumnName("pensumid");

                entity.Property(e => e.Period)
                    .IsRequired()
                    .HasColumnName("period")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Pensum)
                    .WithMany(p => p.Subject)
                    .HasForeignKey(d => d.Pensumid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PensumId_subject");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Email)
                    .HasName("idx_user_email");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.IsBloqued).HasColumnName("isbloqued");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasColumnName("passwordsalt")
                    .HasMaxLength(64);

                entity.Property(e => e.UserRolId)
                    .IsRequired()
                    .HasColumnName("userrolid")
                    .HasMaxLength(25);

                entity.HasOne(d => d.UserRol)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserRolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_userrolid_user");
            });

            modelBuilder.Entity<UserCareer>(entity =>
            {
                entity.ToTable("usercareer");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Careerid).HasColumnName("careerid");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeletionDate).HasColumnName("deletiondate");

                entity.Property(e => e.InstitutionUserId).HasColumnName("institutionuserid");

                entity.Property(e => e.IsDeleted).HasColumnName("isdeleted");

                entity.Property(e => e.PensumId).HasColumnName("pensumid");

                entity.HasOne(d => d.Career)
                    .WithMany(p => p.UserCareer)
                    .HasForeignKey(d => d.Careerid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CareerId_usercareer");

                entity.HasOne(d => d.Pensum)
                    .WithMany(p => p.UserCareers)
                    .HasForeignKey(d => d.PensumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_usercareerId_pensum");

                entity.HasOne(d => d.InstitutionUser)
                    .WithMany(p => p.UserCareer)
                    .HasForeignKey(d => d.InstitutionUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_InstitutionUserId_usercareer");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("userinfo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstname")
                    .HasMaxLength(50);

                entity.Property(e => e.ImagePath)
                    .HasColumnName("imagepath")
                    .HasMaxLength(200);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastname")
                    .HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userinfo)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_UserId_userinfo");
            });

            modelBuilder.Entity<UserLoginDevice>(entity =>
            {
                entity.ToTable("userlogindevice");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationdate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("ip")
                    .HasMaxLength(15);

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(250);

                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userlogindevice)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_UserId_userlogindevice");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("userrole");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(25);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(250);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(75);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
