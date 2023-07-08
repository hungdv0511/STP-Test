using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EikenBadRequest
{
    public string Message { get; set; } = "Validation errors in your request";

    public List<EikenError> Errors { get; set; }
}

public class EikenError
{
    public string Message { get; set; }

    public string Code { get; set; }

    public string Field { get; set; }
}
