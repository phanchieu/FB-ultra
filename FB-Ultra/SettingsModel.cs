using System;
using System.Collections.Generic;
using System.Text;

namespace FB_Ultra
{
    public class SettingsModel
    {
        // Cài đặt chung
        public bool IsMoThuong { get; set; } = true;
        public bool IsMoTuDong { get; set; } = false;
        public string SoLuong { get; set; } = "";
        public string ThoiGianMoLuong { get; set; } = "";
        public string ThoiGianTaiTrang { get; set; } = "";
        public string DelayClickMin { get; set; } = "";
        public string DelayClickMax { get; set; } = "";
        public string HenGio { get; set; } = "0";
        public bool IsNuoiThuong { get; set; } = true;
        public bool IsNuoiAI { get; set; } = false;
        public bool ChanTaiHinhAnh { get; set; } = false;
        public bool TuDongCapNhatCookie { get; set; } = false;
        public bool TuDongVaoNhom { get; set; } = false;
        public bool TuDongDangBaiVaoNhom { get; set; } = false;
        public bool KetBanCheo { get; set; } = false;
        public bool NuoiNickNgauNhien { get; set; } = false;
        public bool likeCmtTheoLink_caiDatChung { get; set; } = false;

        // Tương tác nuôi nick
        public bool LuotNewsfeed { get; set; }
        public bool XemVideoWatch { get; set; }
        public bool XửLyKetBan { get; set; }
        public bool KiemTraThongBao { get; set; }
        public bool BinhLuanBaiViet { get; set; }
        public bool Like { get; set; }
        public bool Tim { get; set; }
        public bool Haha { get; set; }
        public bool Sad { get; set; }
        public bool ThaCamXucNgauNhien { get; set; }
        public string NoiDungTuongTac { get; set; } = "";
        public string ThoiGianNuoiMin { get; set; } = "";
        public string ThoiGianNuoiMax { get; set; } = "";
        public string TiLeThaCamXuc { get; set; } = "0";
        public string TiLeCmt { get; set; } = "0";
        public bool KetBanTheoGoiY { get; set; } = false;

        // Đăng bài spam nhóm
        public bool DangThuong { get; set; } = true;
        public bool DangAnDanh { get; set; } = false;
        public string NoiDungDangBai { get; set; } = "";
        public string LinkFileAnh { get; set; } = "";
        public string DelayDangBai { get; set; } = "";
        public string DanhSachLinkNhomSpam { get; set; } = "";
        public bool DungAi_TuDongDangBai { get; set; } = false;                  // Checkbox Dùng AI
        public string ApiAI_TuDongDangBai { get; set; } = "";          // Ô ST (System Role)
        public string AiSystemPrompt_TuDongDangBai { get; set; } = "";          // Ô ST (System Role)
        public string AiUserPrompt_TuDongDangBai { get; set; } = "";            // Ô User (Content)
        public string TestCapResult_TuDongDangBai { get; set; } = "";           // Ô kết quả Test cap



        // Tự động vào nhóm
        public string DanhSachLinkVaoNhom { get; set; } = "";
        public string DelayVaoNhom { get; set; } = "";

        // Nuôi nick AI
        public bool AiLuotNewsfeed { get; set; }
        public bool AiXemVideoWatch { get; set; }
        public bool AiXuLyKetBan { get; set; }
        public bool AiThaCamXuc { get; set; }
        public bool AiKiemTraThongBao { get; set; }
        public bool AiBinhLuan { get; set; }
        public bool AiTraLoiTinNhan { get; set; }
        public string ApiKeyAi { get; set; } = "";
        public string AiThoiGianNuoiMin { get; set; } = "";
        public string AiThoiGianNuoiMax { get; set; } = "";
        public string AiTiLeThaCamXuc { get; set; } = "0";
        public string AiTiLeCmt { get; set; } = "0";

        // like, cmt theo link
        public bool like_likeCmtTheoLink { get; set; }
        public bool tim_likeCmtTheoLink { get; set; }
        public bool haha_likeCmtTheoLink { get; set; }
        public bool sad_likeCmtTheoLink { get; set; }
        public bool camXucNgauNhien_likeCmtTheoLink { get; set; }
        public bool cmt_likeCmtTheoLink { get; set; }
        public string textCmt_likeCmtTheoLink { get; set; } = "";
        public string link_likeCmtTheoLink { get; set; } = "";
        public string delay_likeCmtTheoLink { get; set; } = "";
    }
}
