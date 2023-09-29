using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;

namespace EC.Manager.Contracts.Dto
{
    [AutoMapTo(typeof(Contract))]
    public class CreatECDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public DateTime? ExpriedTime { get; set; }
    }

    public class ContractBase64ImageDto
    {
        public int ContractPage { get; set; }
        public string FileBase64 { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
    public class CreateContractFromTemplateDto
    {
        public long ContractTemplateId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? ExpriedTime { get; set; }

    }
    public class CreateMassContractDto
    {
        public long Id { get; set; }
        public List<RowMassTemplateExportDto> RowData { get; set; }
    }
}