using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FB_Ultra
{
    public partial class FormSettings : Form
    {
        private SettingsModel currentSettings;

        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            // 1. Load dữ liệu từ file JSON lên
            currentSettings = SettingsManager.LoadSettings();

            // 2. Gán dữ liệu từ currentSettings vào các control tương ứng trên Form

            // cài đặt chung
            rdo_moThuong_caiDatChung.Checked = currentSettings.IsMoThuong;
            rdo_moTuDong_caiDatChung.Checked = currentSettings.IsMoTuDong;
            rdo_nuoiThuong_caiDatChung.Checked = currentSettings.IsNuoiThuong;
            rdo_nuoiAI_caiDatChung.Checked = currentSettings.IsNuoiAI;
            cb_chanTaiHinhAnh_caiDatChung.Checked = currentSettings.ChanTaiHinhAnh;
            cb_tuDongCapNhatCookie_caiDatChung.Checked = currentSettings.TuDongCapNhatCookie;
            cb_ketBanCheo_caiDatChung.Checked = currentSettings.KetBanCheo;
            tb_soLuong_caiDatChung.Text = currentSettings.SoLuong;
            tb_thoiGianMoTungLuong_caiDatChung.Text = currentSettings.ThoiGianMoLuong;
            tb_thoiGianTaiTrang_caiDatChung.Text = currentSettings.ThoiGianTaiTrang;
            tb_thoiGianNghiGiuaCacClick_tu_caiDatChung.Text = currentSettings.DelayClickMin;
            tb_thoiGianNghiGiuaCacClick_den_caiDatChung.Text = currentSettings.DelayClickMax;
            cb_nuoiNickNgauNhien_caiDatChung.Checked = currentSettings.NuoiNickNgauNhien;
            cb_tuVaoNhom_caiDatChung.Checked = currentSettings.TuDongVaoNhom;
            cb_tuDangBaiVaoNhom_caiDatChung.Checked = currentSettings.TuDongDangBaiVaoNhom;

            // tự động đăng bài vào nhóm
            rdo_dangBaiThuong_tuDongDangBaiVaoNhom.Checked = currentSettings.DangThuong;
            rdo_dangBaiAnDanh_tuDongDangBaiVaoNhom.Checked = currentSettings.DangAnDanh;
            tb_noiDungDangBai_tuDongDangBaiVaoNhom.Text = currentSettings.NoiDungDangBai;
            tb_linkAnhDangBai_tuDongDangBaiVaoNhom.Text = currentSettings.LinkFileAnh;
            tb_delayDangBai_tuDongDangBaiVaoNhom.Text = currentSettings.DelayDangBai;
            rtb_linkNhom_tuDongDangBaiVaoNhom.Text = currentSettings.DanhSachLinkNhomSpam;

            // nuôi nick
            cb_luotNewsfeed_nuoiNick.Checked = currentSettings.LuotNewsfeed;
            cb_xemVideo_nuoiNick.Checked = currentSettings.XemVideoWatch;
            cb_xuLyKetBan_nuoiNick.Checked = currentSettings.XửLyKetBan;
            cb_kiemTraThongBaoVaTinNhan_nuoiNick.Checked = currentSettings.KiemTraThongBao;
            cb_like_nuoiNick.Checked = currentSettings.Like;
            cb_tim_nuoiNick.Checked = currentSettings.Tim;
            cb_haha_nuoiNick.Checked = currentSettings.Haha;
            cb_sad_nuoiNick.Checked = currentSettings.Sad;
            cb_thaCamXucNgauNhien_nuoiNick.Checked = currentSettings.ThaCamXucNgauNhien;
            cb_binhLuanBaiViet_nuoiNick.Checked = currentSettings.BinhLuanBaiViet;
            rtb_binhLuan_nuoiNick.Text = currentSettings.NoiDungTuongTac;
            tb_tiLeThaCamXuc_nuoiNick.Text = currentSettings.TiLeThaCamXuc;
            tb_tiLeCmt_nuoiNick.Text = currentSettings.TiLeCmt;
            tb_thoiGianNuoiMin_nuoiNick.Text = currentSettings.ThoiGianNuoiMin;
            tb_thoiGianNuoiMax_nuoiNick.Text = currentSettings.ThoiGianNuoiMax;
            cb_ketBanTheoGoiY_nuoiNick.Checked = currentSettings.KetBanTheoGoiY;

            // tự động vào nhóm
            tb_delay_tuDongVaoNhom.Text = currentSettings.DelayVaoNhom;
            rtb_linkNhom_tuDongVaoNhom.Text = currentSettings.DanhSachLinkVaoNhom;

            // nuôi nick AI
            cb_luotNewsfeed_nuoiNickAI.Checked = currentSettings.AiLuotNewsfeed;
            cb_xemVideo_nuoiNickAI.Checked = currentSettings.AiXemVideoWatch;
            cb_xuLyKetBan_nuoiNickAI.Checked = currentSettings.AiXuLyKetBan;
            cb_kiemTraThongBao_nuoiNickAI.Checked = currentSettings.AiKiemTraThongBao;
            cb_thaCamXucNgauNhien_nuoiNickAI.Checked = currentSettings.AiThaCamXuc;
            cb_binhLuan_nuoiNickAI.Checked = currentSettings.AiBinhLuan;
            cb_traLoiTinNhan_nuoiNickAI.Checked = currentSettings.AiTraLoiTinNhan;
            tb_keyApiAI_nuoiNickAI.Text = currentSettings.ApiKeyAi;
            tb_tiLeThaCamXuc_nuoiNickAI.Text = currentSettings.AiTiLeThaCamXuc;
            tb_tiLeCmt_nuoiNickAI.Text = currentSettings.AiTiLeCmt;
            tb_thoiGianNuoiMin_nuoiNickAI.Text = currentSettings.AiThoiGianNuoiMin;
            tb_thoiGianNuoiMax_nuoiNickAI.Text = currentSettings.AiThoiGianNuoiMax;

            this.ActiveControl = btn_save;
        }

        // Sự kiện khi bấm nút Lưu
        private void btn_save_Click(object sender, EventArgs e)
        {
            // 1. Gom dữ liệu từ các control trên form gán ngược lại vào currentSettings
            // Cài đặt chung
            currentSettings.IsMoThuong = rdo_moThuong_caiDatChung.Checked;
            currentSettings.IsMoTuDong = rdo_moTuDong_caiDatChung.Checked;
            currentSettings.IsNuoiThuong = rdo_nuoiThuong_caiDatChung.Checked;
            currentSettings.IsNuoiAI = rdo_nuoiAI_caiDatChung.Checked;
            currentSettings.ChanTaiHinhAnh = cb_chanTaiHinhAnh_caiDatChung.Checked;
            currentSettings.TuDongCapNhatCookie = cb_tuDongCapNhatCookie_caiDatChung.Checked;
            currentSettings.KetBanCheo = cb_ketBanCheo_caiDatChung.Checked;
            currentSettings.SoLuong = tb_soLuong_caiDatChung.Text;
            currentSettings.ThoiGianMoLuong = tb_thoiGianMoTungLuong_caiDatChung.Text;
            currentSettings.ThoiGianTaiTrang = tb_thoiGianTaiTrang_caiDatChung.Text;
            currentSettings.DelayClickMin = tb_thoiGianNghiGiuaCacClick_tu_caiDatChung.Text;
            currentSettings.DelayClickMax = tb_thoiGianNghiGiuaCacClick_den_caiDatChung.Text;
            currentSettings.NuoiNickNgauNhien = cb_nuoiNickNgauNhien_caiDatChung.Checked;
            currentSettings.TuDongVaoNhom = cb_tuVaoNhom_caiDatChung.Checked;
            currentSettings.TuDongDangBaiVaoNhom = cb_tuDangBaiVaoNhom_caiDatChung.Checked;
            // Tự động đăng bài vào nhóm
            currentSettings.DangThuong = rdo_dangBaiThuong_tuDongDangBaiVaoNhom.Checked;
            currentSettings.DangAnDanh = rdo_dangBaiAnDanh_tuDongDangBaiVaoNhom.Checked;
            currentSettings.NoiDungDangBai = tb_noiDungDangBai_tuDongDangBaiVaoNhom.Text;
            currentSettings.LinkFileAnh = tb_linkAnhDangBai_tuDongDangBaiVaoNhom.Text;
            currentSettings.DelayDangBai = tb_delayDangBai_tuDongDangBaiVaoNhom.Text;
            currentSettings.DanhSachLinkNhomSpam = rtb_linkNhom_tuDongDangBaiVaoNhom.Text;

            // Nuôi nick
            currentSettings.LuotNewsfeed = cb_luotNewsfeed_nuoiNick.Checked;
            currentSettings.XemVideoWatch = cb_xemVideo_nuoiNick.Checked;
            currentSettings.XửLyKetBan = cb_xuLyKetBan_nuoiNick.Checked;
            currentSettings.KiemTraThongBao = cb_kiemTraThongBaoVaTinNhan_nuoiNick.Checked;
            currentSettings.Like = cb_like_nuoiNick.Checked;
            currentSettings.Tim = cb_tim_nuoiNick.Checked;
            currentSettings.Haha = cb_haha_nuoiNick.Checked;
            currentSettings.Sad = cb_sad_nuoiNick.Checked;
            currentSettings.ThaCamXucNgauNhien = cb_thaCamXucNgauNhien_nuoiNick.Checked;
            currentSettings.BinhLuanBaiViet = cb_binhLuanBaiViet_nuoiNick.Checked;
            currentSettings.NoiDungTuongTac = rtb_binhLuan_nuoiNick.Text;
            currentSettings.ThoiGianNuoiMin = tb_thoiGianNuoiMin_nuoiNick.Text;
            currentSettings.ThoiGianNuoiMax = tb_thoiGianNuoiMax_nuoiNick.Text;
            currentSettings.TiLeThaCamXuc = tb_tiLeThaCamXuc_nuoiNick.Text;
            currentSettings.TiLeCmt = tb_tiLeCmt_nuoiNick.Text;
            currentSettings.KetBanTheoGoiY = cb_ketBanTheoGoiY_nuoiNick.Checked;

            // Tự động vào nhóm
            currentSettings.DelayVaoNhom = tb_delay_tuDongVaoNhom.Text;
            currentSettings.DanhSachLinkVaoNhom = rtb_linkNhom_tuDongVaoNhom.Text; // Nhớ check lại tên control cho khớp

            // Nuôi nick AI
            currentSettings.AiLuotNewsfeed = cb_luotNewsfeed_nuoiNickAI.Checked;
            currentSettings.AiXemVideoWatch = cb_xemVideo_nuoiNickAI.Checked;
            currentSettings.AiXuLyKetBan = cb_xuLyKetBan_nuoiNickAI.Checked;
            currentSettings.AiKiemTraThongBao = cb_kiemTraThongBao_nuoiNickAI.Checked;
            currentSettings.AiThaCamXuc = cb_thaCamXucNgauNhien_nuoiNickAI.Checked;
            currentSettings.AiBinhLuan = cb_binhLuan_nuoiNickAI.Checked;
            currentSettings.AiTraLoiTinNhan = cb_traLoiTinNhan_nuoiNickAI.Checked;
            currentSettings.ApiKeyAi = tb_keyApiAI_nuoiNickAI.Text;
            currentSettings.AiThoiGianNuoiMin = tb_thoiGianNuoiMin_nuoiNickAI.Text;
            currentSettings.AiThoiGianNuoiMax = tb_thoiGianNuoiMax_nuoiNickAI.Text;
            currentSettings.AiTiLeThaCamXuc = tb_tiLeThaCamXuc_nuoiNickAI.Text;
            currentSettings.AiTiLeCmt = tb_tiLeCmt_nuoiNickAI.Text;

            // 2. Gọi hàm lưu xuống file JSON
            SettingsManager.SaveSettings(currentSettings);

            MessageBox.Show("Đã lưu cài đặt thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}