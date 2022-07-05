using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASMIK_MAGI.Models
{
    public class PagedList<T>
    {
        public IEnumerable<SelectListItem> Result { get; set; }
        public List<T> Content { get; set; }

        [Required(ErrorMessage = "* Please Fill Kode MO BMRI")]
        public string Kode_MO_BMRI { get; set; }

        [Required(ErrorMessage = "* Please Fill Kode Cabang BMRI")]
        public string Kode_Cabang_BMRI { get; set; }
        public IEnumerable<SelectListItem> Kode_Cabang_BMRI_DDL { get; set; }

        [Required(ErrorMessage = "* Please Fill Nama Cabang BMRI")]
        public string Nama_Cabang_BMRI { get; set; }

        [Required(ErrorMessage = "* Please Fill Product ASMIK")]
        public string Product_ASMIK { get; set; }

        [Required(ErrorMessage = "* Please Fill Nama Lengkap Nasabah")]
        public string Nama_Lengkap_Nasabah { get; set; }

        [Required(ErrorMessage = "* Please Fill No. Rekening Nasabah")]
        public string No_Rekening_Nasabah { get; set; }

        [Required(ErrorMessage = "* Please Fill No. KTP Nasabah")]
        public string No_KTP_Nasabah { get; set; }
        // Tertanggung 1

        public string Nama_Lengkap_Tertanggung_1 { get; set; }

        [Required(ErrorMessage = "* Please Fill Alamat")]
        public string Alamat_1 { get; set; }

        public string Kode_Pos_1 { get; set; }

        [Required(ErrorMessage = "* Please Fill Tempat Lahir")]
        public string Tempat_Lahir_1 { get; set; }

        [Required(ErrorMessage = "* Please Fill Tanggal Lahir")]
        public string Tanggal_Lahir_1 { get; set; }
        public IEnumerable<SelectListItem> Jenis_Kelamin_1_ddl { get; set; }

        [Required(ErrorMessage = "* Please Fill Jenis Kelamin")]
        public string Jenis_Kelamin_1 { get; set; }
        public string No_KTP_1 { get; set; }

        [Required(ErrorMessage = "* Please Fill No. HP")]
        public string No_HP_1 { get; set; }

        [EmailAddress(ErrorMessage = "* Invalid Email Address")]
        public string Email_1 { get; set; }
        public string NPWP_1 { get; set; }
        public string Pekerjaan_1 { get; set; }
        public string Penghasilan_Perbulan_1 { get; set; }
        public string Sumber_penghasilan_1 { get; set; }

        // End Tertanggung 1

        // Tertanggung 2
        public string Nama_Lengkap_Tertanggung_2 { get; set; }
        public string Alamat_2 { get; set; }
        public string Kode_Pos_2 { get; set; }
        public string Tempat_Lahir_2 { get; set; }
        public string Tanggal_Lahir_2 { get; set; }
        public string Jenis_Kelamin_2 { get; set; }
        public string No_KTP_2 { get; set; }
        public string No_HP_2 { get; set; }
        public string Email_2 { get; set; }
        public string NPWP_2 { get; set; }
        public string Pekerjaan_2 { get; set; }
        public string Penghasilan_Perbulan_2{ get; set; }
        public string Sumber_penghasilan_2 { get; set; }
        // End Tertanggung 2

        // Tertanggung 3
        public string Nama_Lengkap_Tertanggung_3 { get; set; }
        public string Alamat_3 { get; set; }
        public string Kode_Pos_3 { get; set; }
        public string Tempat_Lahir_3 { get; set; }
        public string Tanggal_Lahir_3 { get; set; }
        public string Jenis_Kelamin_3 { get; set; }
        public string No_KTP_3 { get; set; }
        public string No_HP_3 { get; set; }
        public string Email_3 { get; set; }
        public string NPWP_3 { get; set; }
        public string Pekerjaan_3 { get; set; }
        public string Penghasilan_Perbulan_3 { get; set; }
        public string Sumber_penghasilan_3 { get; set; }
        // End Tertanggung 3

        // Tertanggung 4
        public string Nama_Lengkap_Tertanggung_4 { get; set; }
        public string Alamat_4 { get; set; }
        public string Kode_Pos_4 { get; set; }
        public string Tempat_Lahir_4 { get; set; }
        public string Tanggal_Lahir_4 { get; set; }
        public string Jenis_Kelamin_4 { get; set; }
        public string No_KTP_4 { get; set; }
        public string No_HP_4 { get; set; }
        public string Email_4 { get; set; }
        public string NPWP_4 { get; set; }
        public string Pekerjaan_4 { get; set; }
        public string Penghasilan_Perbulan_4 { get; set; }
        public string Sumber_penghasilan_4 { get; set; }
        // End Tertanggung 4

        // Tertanggung 5
        public string Nama_Lengkap_Tertanggung_5 { get; set; }
        public string Alamat_5 { get; set; }
        public string Kode_Pos_5 { get; set; }
        public string Tempat_Lahir_5 { get; set; }
        public string Tanggal_Lahir_5 { get; set; }
        public string Jenis_Kelamin_5 { get; set; }
        public string No_KTP_5 { get; set; }
        public string No_HP_5 { get; set; }
        public string Email_5 { get; set; }
        public string NPWP_5 { get; set; }
        public string Pekerjaan_5 { get; set; }
        public string Penghasilan_Perbulan_5 { get; set; }
        public string Sumber_penghasilan_5 { get; set; }
        // End Tertanggung 5

        // List Daftar Tertanggung
        public string Nama_Lengkap_Tertanggung_List { get; set; }
        public string Alamat_List { get; set; }
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
        // End List Daftar Tertanggung

        // AHLI WARIS

        public string Nama_Lengkap_AHLI_WARIS { get; set; }
        public string Alamat_AHLI_WARIS { get; set; }

        public IEnumerable<SelectListItem> HUB_DGN_NASABAH_DDL { get; set; }
        public string HUB_DGN_NASABAH { get; set; }
        public string IDX_ASMIK { get; set; }
        public string COUNTING_IDX_ASMIK { get; set; }
        public string IDX_ASMIK_DETAIL { get; set; }
        public string IDX_HEADER { get; set; }
        public string ReffNo_ASMIK { get; set; }
        public string KTP_NO_SEARCHING { get; set; }
        public string LINK { get; set; }
        public string NomerUrut { get; set; }

        public string Status { get; set; }
        public bool? Disabled { get; set; }
        public bool? DisabledSubmit { get; set; }
        public bool? DisabledDraft { get; set; }
        public string Visible { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        [Range(0, 5)]
        public int JumlahPolis { get; set; }
        // END AHLI WARIS
    }
}