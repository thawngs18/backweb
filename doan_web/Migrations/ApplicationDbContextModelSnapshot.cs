﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using doan_web.Models;

#nullable disable

namespace doan_web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("doan_web.Models.ChiTietDonHang", b =>
                {
                    b.Property<int>("MaDonHang")
                        .HasColumnType("int");

                    b.Property<int>("MaSP")
                        .HasColumnType("int");

                    b.Property<decimal>("DonGia")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("MaDonHang", "MaSP");

                    b.HasIndex("MaSP");

                    b.ToTable("ChiTietDonHang");
                });

            modelBuilder.Entity("doan_web.Models.ChiTietGioHang", b =>
                {
                    b.Property<int>("MaGioHang")
                        .HasColumnType("int");

                    b.Property<int>("MaSP")
                        .HasColumnType("int");

                    b.Property<decimal>("DonGia")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("MaGioHang", "MaSP");

                    b.HasIndex("MaSP");

                    b.ToTable("ChiTietGioHang");
                });

            modelBuilder.Entity("doan_web.Models.DonHang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DiaChi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MaKH")
                        .HasColumnType("int");

                    b.Property<int?>("MaNV")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayDat")
                        .HasColumnType("datetime2");

                    b.Property<string>("QuanHuyen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SoDienThoai")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<string>("ThanhPho")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TongTien")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("MaKH");

                    b.HasIndex("MaNV");

                    b.ToTable("DonHang");
                });

            modelBuilder.Entity("doan_web.Models.GioHang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MaKH")
                        .HasColumnType("int");

                    b.Property<decimal>("SoTien")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("MaKH")
                        .IsUnique()
                        .HasFilter("[MaKH] IS NOT NULL");

                    b.ToTable("GioHang");
                });

            modelBuilder.Entity("doan_web.Models.KhachHang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DiaChi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MatKhau")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SDT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaiKhoan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ten")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("KhachHang");
                });

            modelBuilder.Entity("doan_web.Models.LoaiSanPham", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ten")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LoaiSanPham");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Ten = "Món Chính"
                        },
                        new
                        {
                            Id = 2,
                            Ten = "Khai Vị"
                        },
                        new
                        {
                            Id = 3,
                            Ten = "Tráng Miệng"
                        },
                        new
                        {
                            Id = 4,
                            Ten = "Đồ Uống"
                        });
                });

            modelBuilder.Entity("doan_web.Models.NhanVien", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DiaChi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MatKhau")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SDT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaiKhoan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ten")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NhanVien");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            MatKhau = "123",
                            TaiKhoan = "admin"
                        });
                });

            modelBuilder.Entity("doan_web.Models.PhanHoi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MaKH")
                        .HasColumnType("int");

                    b.Property<int?>("MaSP")
                        .HasColumnType("int");

                    b.Property<DateTime?>("NgayBinhLuan")
                        .HasColumnType("datetime2");

                    b.Property<string>("NoiDung")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SoSao")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MaKH");

                    b.HasIndex("MaSP");

                    b.ToTable("PhanHoi");
                });

            modelBuilder.Entity("doan_web.Models.SanPham", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("GiaBan")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("IDLoaiSP")
                        .HasColumnType("int");

                    b.Property<string>("MoTa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.Property<string>("Ten")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("img")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IDLoaiSP");

                    b.ToTable("SanPham");
                });

            modelBuilder.Entity("doan_web.Models.ChiTietDonHang", b =>
                {
                    b.HasOne("doan_web.Models.DonHang", "DonHang")
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("MaDonHang")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("doan_web.Models.SanPham", "SanPham")
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("MaSP")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DonHang");

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("doan_web.Models.ChiTietGioHang", b =>
                {
                    b.HasOne("doan_web.Models.GioHang", "GioHang")
                        .WithMany("ChiTietGioHangs")
                        .HasForeignKey("MaGioHang")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("doan_web.Models.SanPham", "SanPham")
                        .WithMany("ChiTietGioHangs")
                        .HasForeignKey("MaSP")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("GioHang");

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("doan_web.Models.DonHang", b =>
                {
                    b.HasOne("doan_web.Models.KhachHang", "KhachHang")
                        .WithMany("DonHangs")
                        .HasForeignKey("MaKH")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("doan_web.Models.NhanVien", "NhanVien")
                        .WithMany("DonHangs")
                        .HasForeignKey("MaNV")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("KhachHang");

                    b.Navigation("NhanVien");
                });

            modelBuilder.Entity("doan_web.Models.GioHang", b =>
                {
                    b.HasOne("doan_web.Models.KhachHang", "KhachHang")
                        .WithOne("GioHang")
                        .HasForeignKey("doan_web.Models.GioHang", "MaKH")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("KhachHang");
                });

            modelBuilder.Entity("doan_web.Models.PhanHoi", b =>
                {
                    b.HasOne("doan_web.Models.KhachHang", "KhachHang")
                        .WithMany("PhanHois")
                        .HasForeignKey("MaKH")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("doan_web.Models.SanPham", "SanPham")
                        .WithMany("PhanHois")
                        .HasForeignKey("MaSP")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("KhachHang");

                    b.Navigation("SanPham");
                });

            modelBuilder.Entity("doan_web.Models.SanPham", b =>
                {
                    b.HasOne("doan_web.Models.LoaiSanPham", "LoaiSanPham")
                        .WithMany("SanPhams")
                        .HasForeignKey("IDLoaiSP")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("LoaiSanPham");
                });

            modelBuilder.Entity("doan_web.Models.DonHang", b =>
                {
                    b.Navigation("ChiTietDonHangs");
                });

            modelBuilder.Entity("doan_web.Models.GioHang", b =>
                {
                    b.Navigation("ChiTietGioHangs");
                });

            modelBuilder.Entity("doan_web.Models.KhachHang", b =>
                {
                    b.Navigation("DonHangs");

                    b.Navigation("GioHang");

                    b.Navigation("PhanHois");
                });

            modelBuilder.Entity("doan_web.Models.LoaiSanPham", b =>
                {
                    b.Navigation("SanPhams");
                });

            modelBuilder.Entity("doan_web.Models.NhanVien", b =>
                {
                    b.Navigation("DonHangs");
                });

            modelBuilder.Entity("doan_web.Models.SanPham", b =>
                {
                    b.Navigation("ChiTietDonHangs");

                    b.Navigation("ChiTietGioHangs");

                    b.Navigation("PhanHois");
                });
#pragma warning restore 612, 618
        }
    }
}
