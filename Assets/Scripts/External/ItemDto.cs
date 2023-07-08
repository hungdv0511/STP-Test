using System;
using System.Collections.Generic;

public class ItemDto
{
    public string ItemId { get; set; }

    public float duration { get; set; }

    public DateTime useTime { get; set; }

    public DateTime expiredDate { get; set; }
}


public class ListItemDto
{
    public List<ItemDto> data { get; set; }

    public DateTime currentTime { get; set; }
}