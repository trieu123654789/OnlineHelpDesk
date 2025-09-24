using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace OnlineHelpDesk2.Models
{
    public partial class OHDDBContext : DbContext
    {
        public OHDDBContext()
            : base("name=OHDDBContext")
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<CloseRequest> CloseRequests { get; set; }
        public virtual DbSet<Facility> Facilities { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<GuestLetter> GuestLetters { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<SummaryReport> SummaryReports { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Fullname)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Gender)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.Requests)
                .WithOptional(e => e.Account)
                .HasForeignKey(e => e.AssigneeID);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.Requests1)
                .WithRequired(e => e.Account1)
                .HasForeignKey(e => e.RequestorID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CloseRequest>()
                .Property(e => e.Reason)
                .IsUnicode(false);

            modelBuilder.Entity<Facility>()
                .Property(e => e.FacilityName)
                .IsUnicode(false);

            modelBuilder.Entity<Facility>()
                .HasMany(e => e.Accounts)
                .WithRequired(e => e.Facility)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Facility>()
                .HasMany(e => e.Requests)
                .WithRequired(e => e.Facility)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Feedback>()
                .Property(e => e.FeedbackContent)
                .IsUnicode(false);

            modelBuilder.Entity<Feedback>()
                .HasMany(e => e.SummaryReports)
                .WithRequired(e => e.Feedback)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<GuestLetter>()
                .Property(e => e.LetterContent)
                .IsUnicode(false);

            modelBuilder.Entity<GuestLetter>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<GuestLetter>()
                .Property(e => e.Mail)
                .IsUnicode(false);

            modelBuilder.Entity<Reply>()
                .Property(e => e.ReplyContent)
                .IsUnicode(false);

            modelBuilder.Entity<Reply>()
                .HasMany(e => e.SummaryReports)
                .WithRequired(e => e.Reply)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Report>()
                .Property(e => e.Month)
                .IsUnicode(false);

            modelBuilder.Entity<Report>()
                .HasMany(e => e.SummaryReports)
                .WithRequired(e => e.Report)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.RequestContent)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.SeverityLevels)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .HasMany(e => e.CloseRequests)
                .WithRequired(e => e.Request)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Request>()
                .HasMany(e => e.Feedbacks)
                .WithRequired(e => e.Request)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Request>()
                .HasMany(e => e.Replies)
                .WithRequired(e => e.Request)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Request>()
                .HasMany(e => e.SummaryReports)
                .WithRequired(e => e.Request)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserType>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<UserType>()
                .HasMany(e => e.Accounts)
                .WithRequired(e => e.UserType)
                .WillCascadeOnDelete(false);
        }
    }
}
