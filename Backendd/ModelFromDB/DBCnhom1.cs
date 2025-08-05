using System;
using System.Collections.Generic;
using Backendd.Dtos;
using Backendd.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

public partial class DBCnhom1 : DbContext
{
    public DBCnhom1()
    {
    }

    public DBCnhom1(DbContextOptions<DBCnhom1> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCamera> TblCameras { get; set; }

    public virtual DbSet<TblClass> TblClasses { get; set; }

    public virtual DbSet<TblJwtblacklist> TblJwtblacklists { get; set; }

    public virtual DbSet<TblLog> TblLogs { get; set; }

    public virtual DbSet<TblResponse> TblResponses { get; set; }

    public virtual DbSet<TblSchedule> TblSchedules { get; set; }

    public virtual DbSet<TblScheduleUser> TblScheduleUsers { get; set; }

    public virtual DbSet<TblSubject> TblSubjects { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TimeTableDto> TimeTableDtos { get; set; }

    public DbSet<StudentForModalDto> StudentForModalDtos { get; set; }
    public virtual DbSet<DTO_TimeTableForStudent> TimeTableForStudentDtos { get; set; }
    public virtual DbSet<DTO_StudentsList> DTO_StudentsLists { get; set; }
    public virtual DbSet<DTO_TeachersList> DTO_TeachersLists { get; set; }
    public virtual DbSet<DTO_Schedule> DTO_Schedules { get; set; }
    public virtual DbSet<UnknownLogDto> UnknownLogDto { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TimeTableDto>().HasNoKey().Property(t => t.ScId).HasColumnName("sc_id");
        modelBuilder.Entity<StudentForModalDto>().HasNoKey();
        modelBuilder.Entity<TimeTableDto>().HasNoKey();
        modelBuilder.Entity<DTO_StudentsList>().HasNoKey();
        modelBuilder.Entity<DTO_TeachersList>().HasNoKey();
        modelBuilder.Entity<DTO_TimeTableForStudent>().HasNoKey();
        modelBuilder.Entity<DTO_Schedule>().HasNoKey();
        modelBuilder.Entity<UpdateAttendanceDto>().HasNoKey();
        modelBuilder.Entity<UnknownLogDto>().HasNoKey();
        modelBuilder.Entity<TblCamera>()
            .HasKey(c => c.CamId);

        modelBuilder.Entity<TblClass>()
            .HasKey(c => c.ClId);

        modelBuilder.Entity<TblJwtblacklist>()
            .HasKey(j => j.JwtId);

        modelBuilder.Entity<TblLog>()
            .HasKey(l => l.LogId);

        modelBuilder.Entity<TblResponse>()
            .HasKey(r => r.ResId);

        modelBuilder.Entity<TblSchedule>()
            .HasKey(s => s.ScId);

        modelBuilder.Entity<TblScheduleUser>()
            .HasKey(su => new { su.SuScheduleid, su.SuUserid });

        modelBuilder.Entity<TblSubject>()
            .HasKey(s => s.SbId);

        modelBuilder.Entity<TblUser>()
            .HasKey(u => u.UsId);

        modelBuilder.Entity<TblSchedule>()
            .HasOne(s => s.ScSubject)
            .WithMany(sb => sb.TblSchedules)
            .HasForeignKey(s => s.ScSubjectid);

        modelBuilder.Entity<TblSchedule>()
            .HasOne(s => s.Class)
            .WithMany(c => c.TblSchedules)
            .HasForeignKey(s => s.ScClassId);

        modelBuilder.Entity<TblScheduleUser>()
            .HasOne(su => su.SuSchedule)
            .WithMany(s => s.TblScheduleUsers)
            .HasForeignKey(su => su.SuScheduleid);

        modelBuilder.Entity<TblScheduleUser>()
            .HasOne(su => su.SuUser)
            .WithMany(u => u.TblScheduleUsers)
            .HasForeignKey(su => su.SuUserid);
    }

}
