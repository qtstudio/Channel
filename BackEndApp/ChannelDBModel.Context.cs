﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BackendApp
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ChannelDBEntities : DbContext
    {
        public ChannelDBEntities()
            : base("name=ChannelDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AppAd> AppAds { get; set; }
        public DbSet<AppChannel> AppChannels { get; set; }
        public DbSet<AppImage> AppImages { get; set; }
        public DbSet<AppInfo> AppInfoes { get; set; }
        public DbSet<AppTag> AppTags { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterApp> CharacterApps { get; set; }
        public DbSet<CharacterTag> CharacterTags { get; set; }
        public DbSet<ColorScheme> ColorSchemes { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}