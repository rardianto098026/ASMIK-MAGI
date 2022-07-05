using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASMIK_MAGI.Models
{
    public class ASMIKModels
    {
        public string Kode_MO_BMRI { get; set; }


        // List Daftar Tertanggung
        public string Nama_Lengkap_Tertanggung_List { get; set; }
        public string ReffNo_List { get; set; }
        public string Alamat_List { get; set; }
        public string Status { get; set; }
        public string Kode_Pos_List { get; set; }
        public string Tempat_Lahir_List { get; set; }
        public string Tanggal_Lahir_List { get; set; }
        public string Jenis_Kelamin_List { get; set; }
        public string No_KTP_List { get; set; }
        public string No_HP_List { get; set; }
        public string Email_List { get; set; }
        public string NPWP_List { get; set; }
        public string Pekerjaan_List { get; set; }
        public string Penghasilan_Perbulan_List { get; set; }
        public string Sumber_penghasilan_List { get; set; }
        public string IDX_ASMIK_DETAIL { get; set; }
        public string IDX_ASMIK { get; set; }
        public string IDX_HEADER { get; set; }
        public string Kode_Cabang_BMRI { get; set; }
        public string Nama_Cabang_BMRI { get; set; }
        public string Product_ASMIK { get; set; }
        public string Nama_Lengkap_Nasabah { get; set; }
        public string No_Rekening_Nasabah { get; set; }
        public string No_KTP_Nasabah { get; set; }
        public string NomerUrut { get; set; }
        public bool? Disabled { get; set; }
        public bool? DisabledSubmit { get; set; }
        public bool? DisabledDraft { get; set; }
        public string Visible { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string No { get; set; }
        public string Idx_login { get; set; }
        // End List Daftar Tertanggung

    }
}