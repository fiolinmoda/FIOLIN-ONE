using FiolinOne.Domain.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class CmsPageConfiguration : IEntityTypeConfiguration<CmsPage>
{
    public void Configure(EntityTypeBuilder<CmsPage> builder)
    {
        builder.ToTable("cms_pages");
        builder.HasKey(page => page.Id);
        builder.Property(page => page.Id).HasColumnName("id");
        builder.Property(page => page.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(page => page.Slug).HasColumnName("slug").HasMaxLength(200).IsRequired();
        builder.Property(page => page.Content).HasColumnName("content").IsRequired();
        builder.Property(page => page.IsPublished).HasColumnName("is_published").IsRequired();
        builder.Property(page => page.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(page => page.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(page => page.UpdatedAtUtc).HasColumnName("updated_at");
        builder.HasIndex(page => page.Slug).IsUnique();
    }
}

public sealed class CmsMenuConfiguration : IEntityTypeConfiguration<CmsMenu>
{
    public void Configure(EntityTypeBuilder<CmsMenu> builder)
    {
        builder.ToTable("cms_menus");
        builder.HasKey(menu => menu.Id);
        builder.Property(menu => menu.Id).HasColumnName("id");
        builder.Property(menu => menu.Title).HasColumnName("title").HasMaxLength(120).IsRequired();
        builder.Property(menu => menu.Url).HasColumnName("url").HasMaxLength(300).IsRequired();
        builder.Property(menu => menu.Location).HasColumnName("location").HasMaxLength(80).IsRequired();
        builder.Property(menu => menu.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(menu => menu.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(menu => menu.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(menu => menu.UpdatedAtUtc).HasColumnName("updated_at");
        builder.HasIndex(menu => new { menu.Location, menu.SortOrder });
    }
}

public sealed class CmsBannerConfiguration : IEntityTypeConfiguration<CmsBanner>
{
    public void Configure(EntityTypeBuilder<CmsBanner> builder)
    {
        builder.ToTable("cms_banners");
        builder.HasKey(banner => banner.Id);
        builder.Property(banner => banner.Id).HasColumnName("id");
        builder.Property(banner => banner.Title).HasColumnName("title").HasMaxLength(180).IsRequired();
        builder.Property(banner => banner.Subtitle).HasColumnName("subtitle").HasMaxLength(240);
        builder.Property(banner => banner.ImageUrl).HasColumnName("image_url").HasMaxLength(600).IsRequired();
        builder.Property(banner => banner.LinkUrl).HasColumnName("link_url").HasMaxLength(300);
        builder.Property(banner => banner.Placement).HasColumnName("placement").HasMaxLength(80).IsRequired();
        builder.Property(banner => banner.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(banner => banner.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(banner => banner.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(banner => banner.UpdatedAtUtc).HasColumnName("updated_at");
        builder.HasIndex(banner => new { banner.Placement, banner.SortOrder });
    }
}

public sealed class CmsSliderConfiguration : IEntityTypeConfiguration<CmsSlider>
{
    public void Configure(EntityTypeBuilder<CmsSlider> builder)
    {
        builder.ToTable("cms_sliders");
        builder.HasKey(slider => slider.Id);
        builder.Property(slider => slider.Id).HasColumnName("id");
        builder.Property(slider => slider.Title).HasColumnName("title").HasMaxLength(180).IsRequired();
        builder.Property(slider => slider.Subtitle).HasColumnName("subtitle").HasMaxLength(240);
        builder.Property(slider => slider.ImageUrl).HasColumnName("image_url").HasMaxLength(600).IsRequired();
        builder.Property(slider => slider.LinkUrl).HasColumnName("link_url").HasMaxLength(300);
        builder.Property(slider => slider.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(slider => slider.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(slider => slider.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(slider => slider.UpdatedAtUtc).HasColumnName("updated_at");
        builder.HasIndex(slider => slider.SortOrder);
    }
}

public sealed class CmsSettingConfiguration : IEntityTypeConfiguration<CmsSetting>
{
    public void Configure(EntityTypeBuilder<CmsSetting> builder)
    {
        builder.ToTable("cms_settings");
        builder.HasKey(setting => setting.Id);
        builder.Property(setting => setting.Id).HasColumnName("id");
        builder.Property(setting => setting.Key).HasColumnName("key").HasMaxLength(120).IsRequired();
        builder.Property(setting => setting.Value).HasColumnName("value").HasMaxLength(1000).IsRequired();
        builder.Property(setting => setting.Group).HasColumnName("group").HasMaxLength(80).IsRequired();
        builder.Property(setting => setting.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(setting => setting.UpdatedAtUtc).HasColumnName("updated_at");
        builder.HasIndex(setting => setting.Key).IsUnique();
    }
}

public sealed class CmsSeoPageConfiguration : IEntityTypeConfiguration<CmsSeoPage>
{
    public void Configure(EntityTypeBuilder<CmsSeoPage> builder)
    {
        builder.ToTable("cms_seo_pages");
        builder.HasKey(seo => seo.Id);
        builder.Property(seo => seo.Id).HasColumnName("id");
        builder.Property(seo => seo.Route).HasColumnName("route").HasMaxLength(240).IsRequired();
        builder.Property(seo => seo.MetaTitle).HasColumnName("meta_title").HasMaxLength(180).IsRequired();
        builder.Property(seo => seo.MetaDescription).HasColumnName("meta_description").HasMaxLength(320).IsRequired();
        builder.Property(seo => seo.Canonical).HasColumnName("canonical").HasMaxLength(400);
        builder.Property(seo => seo.OpenGraphTitle).HasColumnName("open_graph_title").HasMaxLength(180);
        builder.Property(seo => seo.OpenGraphDescription).HasColumnName("open_graph_description").HasMaxLength(320);
        builder.Property(seo => seo.TwitterCard).HasColumnName("twitter_card").HasMaxLength(80);
        builder.Property(seo => seo.SchemaJson).HasColumnName("schema_json");
        builder.Property(seo => seo.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(seo => seo.UpdatedAtUtc).HasColumnName("updated_at");
        builder.HasIndex(seo => seo.Route).IsUnique();
    }
}
