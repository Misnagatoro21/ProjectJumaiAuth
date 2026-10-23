namespace ProjectJumaiAuth.DTOS
{
    public class DataTableRequestDto
    {

        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSearchDto? Search { get; set; }
        public List<DataTableOrderDto> Order { get; set; } = [];
        public List<DataTableColumnDto> Columns { get; set; } = [];
    }

    public class DataTableSearchDto
    {
        public string? Value { get; set; }
    }

    public class DataTableOrderDto
    {
        public int Column { get; set; }
        public string Dir { get; set; } = "asc";
    }

    public class DataTableColumnDto
    {
        public string? Data { get; set; }
    }
}
