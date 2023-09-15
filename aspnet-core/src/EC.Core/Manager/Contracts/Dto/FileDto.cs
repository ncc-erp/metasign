using Abp.Application.Services.Dto;
using NccCore.Anotations;
using System;

namespace EC.Manager.Contracts.Dto
{
    public class FileDto
    {
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
    }
}