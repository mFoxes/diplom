﻿namespace GrandmaApi.Models.Configs;

public class EmailNotifierConfig
{
    public string SoonTemplateName { get; set; }
    public string OverdueTemplateName { get; set; }
    public string AdminNotificationTemplateName { get; set; }
    public string DeletedUserNotificationTemplateName { get; set; }
}