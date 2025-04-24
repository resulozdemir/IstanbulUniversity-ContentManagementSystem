using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

public partial class UCmsContext : DbContext
{
    public UCmsContext()
    {
    }

    public UCmsContext(DbContextOptions<UCmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAksisUnvan> TAksisUnvans { get; set; }

    public virtual DbSet<TAppComponent> TAppComponents { get; set; }

    public virtual DbSet<TAppContentgroup> TAppContentgroups { get; set; }

    public virtual DbSet<TAppContentpage> TAppContentpages { get; set; }

    public virtual DbSet<TAppContentpage200429> TAppContentpage2s { get; set; }

    public virtual DbSet<TAppContentpage200429> TAppContentpage200429s { get; set; }

    public virtual DbSet<TAppContentpage201215> TAppContentpage201215s { get; set; }

    public virtual DbSet<TAppContentpage201216> TAppContentpage201216s { get; set; }

    public virtual DbSet<TAppEvent> TAppEvents { get; set; }

    public virtual DbSet<TAppForm> TAppForms { get; set; }

    public virtual DbSet<TAppJsontree> TAppJsontrees { get; set; }

    public virtual DbSet<TAppLink> TAppLinks { get; set; }

    public virtual DbSet<TAppMenu> TAppMenus { get; set; }

    public virtual DbSet<TAppNews> TAppNews { get; set; }

    public virtual DbSet<TAppNotice> TAppNotices { get; set; }

    public virtual DbSet<TAppProjectNews> TAppProjectNews { get; set; }

    public virtual DbSet<TAppSite> TAppSites { get; set; }

    public virtual DbSet<TAppSitecomponentdata20231024> TAppSitecomponentdata20231024s { get; set; }

    public virtual DbSet<TAppSitecomponentdataTemp> TAppSitecomponentdataTemps { get; set; }

    public virtual DbSet<TAppSitecomponentdataTwitterIcon> TAppSitecomponentdataTwitterIcons { get; set; }

    public virtual DbSet<TAppSitecomponentdataYedek> TAppSitecomponentdataYedeks { get; set; }

    public virtual DbSet<TAppSitecomponentdata> TAppSitecomponentdata { get; set; }

    public virtual DbSet<TAppSitecss> TAppSitecsses { get; set; }

    public virtual DbSet<TAppSitedomain> TAppSitedomains { get; set; }

    public virtual DbSet<TAppSitemap> TAppSitemaps { get; set; }

    public virtual DbSet<TAppSitepage> TAppSitepages { get; set; }

    public virtual DbSet<TAppSiteunit> TAppSiteunits { get; set; }

    public virtual DbSet<TAppSnapshotOld> TAppSnapshotOlds { get; set; }

    public virtual DbSet<TAppTheme> TAppThemes { get; set; }

    public virtual DbSet<TAppThemecomponent> TAppThemecomponents { get; set; }

    public virtual DbSet<TAppTranslate> TAppTranslates { get; set; }

    public virtual DbSet<TAppUpload> TAppUploads { get; set; }

    public virtual DbSet<TAppUploadapp> TAppUploadapps { get; set; }

    public virtual DbSet<TAppUploadfile> TAppUploadfiles { get; set; }

    public virtual DbSet<TAppViewcount> TAppViewcounts { get; set; }

    public virtual DbSet<TAuthLogin> TAuthLogins { get; set; }

    public virtual DbSet<TAuthRole> TAuthRoles { get; set; }

    public virtual DbSet<TAuthUser> TAuthUsers { get; set; }

    public virtual DbSet<TAuthUserinrole> TAuthUserinroles { get; set; }

    public virtual DbSet<TAvesProfile> TAvesProfiles { get; set; }

    public virtual DbSet<TPbysTree> TPbysTrees { get; set; }

    public virtual DbSet<TPbysYonetselgrup> TPbysYonetselgrups { get; set; }

    public virtual DbSet<TSpcDeploy> TSpcDeploys { get; set; }

    public virtual DbSet<TempEvent> TempEvents { get; set; }

    public virtual DbSet<TempGalleryIdRef> TempGalleryIdRefs { get; set; }

    public virtual DbSet<TempTransfer> TempTransfers { get; set; }

    public virtual DbSet<VAksisDer> VAksisDers { get; set; }

    public virtual DbSet<VAksisDokuman> VAksisDokumen { get; set; }

    public virtual DbSet<VAksisTree> VAksisTrees { get; set; }

    public virtual DbSet<VAppSearch> VAppSearches { get; set; }

    public virtual DbSet<VAppViewcount> VAppViewcounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=RESULOZDEMIR;Database=U_CMS;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAksisUnvan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_AKSIS_UNVAN_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppComponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_COMPONENT_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppContentgroup>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppContentgroups).HasConstraintName("FK_CONTENTGROUP_SITE");
        });

        modelBuilder.Entity<TAppContentpage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_CONTENTPAGE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Group).WithMany(p => p.TAppContentpages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CONTENTPAGE_CONTENTGROUP");

            entity.HasOne(d => d.Site).WithMany(p => p.TAppContentpages).HasConstraintName("FK_CONTENTPAGE_SITE");
        });

        modelBuilder.Entity<TAppEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_EVENT_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EVENT_SITE");
        });

        modelBuilder.Entity<TAppForm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_FORM_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppJsontree>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_JSONTREE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_CMS_LINK_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppMenu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_MENU_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_MENU_PARENT");

            entity.HasOne(d => d.Site).WithMany(p => p.TAppMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MENU_SITE");
        });

        modelBuilder.Entity<TAppNews>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_NEWS_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppNews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NEWS_SITE");
        });

        modelBuilder.Entity<TAppNotice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_NOTICE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Categoryid).HasDefaultValue(1);
        });

        modelBuilder.Entity<TAppProjectNews>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_PROJECT_NEWS_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppProjectNews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PROJECT_NEWS_SITE");
        });

        modelBuilder.Entity<TAppSite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_SITE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Subunitpbys).HasDefaultValue(1);
        });

        modelBuilder.Entity<TAppSitecomponentdata>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_SITECOMPONENTDATA_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppSitecomponentdata)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SITECOMPONENTDATA_SITE");

            entity.HasOne(d => d.Themecomponent).WithMany(p => p.TAppSitecomponentdata)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SITECOMPONENTDATA_THEMECOMPONENT");
        });

        modelBuilder.Entity<TAppSitecss>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_SITECSS_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppSitedomain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_SITEDOMAIN_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppSitedomains)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SITEDOMAIN_SITE");
        });

        modelBuilder.Entity<TAppSitemap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_SNAPSHOT_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Active).HasDefaultValue(1);
        });

        modelBuilder.Entity<TAppSitepage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_SITEPAGE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppSitepages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SITEPAGE_SITE");
        });

        modelBuilder.Entity<TAppSnapshotOld>(entity =>
        {
            entity.HasKey(e => e.Urlhash).HasName("PK_SNAPSHOT");
        });

        modelBuilder.Entity<TAppTheme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_THEME_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppThemecomponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_THEMECOMPONENT_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Theme).WithMany(p => p.TAppThemecomponents).HasConstraintName("FK_THEMECOMPONENT_THEME");
        });

        modelBuilder.Entity<TAppTranslate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_TRANSLATE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_UPLOAD_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppUploads)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UPLOAD_SITE");

            entity.HasOne(d => d.User).WithMany(p => p.TAppUploads)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UPLOAD_USER");
        });

        modelBuilder.Entity<TAppUploadapp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_APP_UPLOADAPP_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAppUploadfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__T_APP_UP__3214EC274DCAD445");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Site).WithMany(p => p.TAppUploadfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UPLOADFILE_SITE");

            entity.HasOne(d => d.User).WithMany(p => p.TAppUploadfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UPLOADFILE_USER");
        });

        modelBuilder.Entity<TAuthLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_AUTH_LOGIN_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.TAuthLogins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LOGIN_USER");
        });

        modelBuilder.Entity<TAuthRole>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAuthUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_AUTH_USER_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<TAuthUserinrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_AUTH_USERINROLE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Role).WithMany(p => p.TAuthUserinroles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERINROLE_ROLE");

            entity.HasOne(d => d.User).WithMany(p => p.TAuthUserinroles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERINROLE_USER");
        });

        modelBuilder.Entity<TAvesProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_AVES_PROFILE_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.KimliknoNavigation).WithMany(p => p.TAvesProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AVES_PROFILE_USER");
        });

        modelBuilder.Entity<TPbysTree>(entity =>
        {
            entity.Property(e => e.Birimkod).ValueGeneratedNever();
            entity.Property(e => e.Isdeleted).HasDefaultValue("0");
        });

        modelBuilder.Entity<TPbysYonetselgrup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TABLE1_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Birim).WithMany(p => p.TPbysYonetselgrups).HasConstraintName("FK_YONETSELGRUP_TREE");
        });

        modelBuilder.Entity<TSpcDeploy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("T_SPC_DEPLOY_PK");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
