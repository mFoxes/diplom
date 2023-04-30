using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandmaApi.Models.Configs;
using Microsoft.Extensions.Options;
using Scriban;

namespace GrandmaApi.Notification
{
    public class TemplateProvider
    {
        private readonly IOptions<EmailNotifierConfig> _emailNotifierConfig;
        private readonly IOptions<MattermostNotifierConfig> _mattermostNotifierConfig;
        public TemplateProvider(IOptions<EmailNotifierConfig> emailNotifierConfig, IOptions<MattermostNotifierConfig> mattermostNotifierConfig)
        {
            _emailNotifierConfig = emailNotifierConfig;
            _mattermostNotifierConfig = mattermostNotifierConfig;
        }

        public Template GetSoonTemplateForEmail()
        {
            var template = File.ReadAllText(_emailNotifierConfig.Value.SoonTemplateName);
            return Template.Parse(template);
        }
        public Template GetSoonTemplateForMattermost()
        {
            var template = File.ReadAllText(_mattermostNotifierConfig.Value.SoonTemplateName);
            return Template.Parse(template);
        }
        public Template GetOverdueTemplateForEmail()
        {
            var template = File.ReadAllText(_emailNotifierConfig.Value.OverdueTemplateName);
            return Template.Parse(template);
        }
        public Template GetOverdueTemplateForMattermost()
        {
            var template = File.ReadAllText(_mattermostNotifierConfig.Value.OverdueTemplateName);
            return Template.Parse(template);
        }
        public Template GetOverdueAdminsTemplateForEmail()
        {
            var template = File.ReadAllText(_emailNotifierConfig.Value.AdminNotificationTemplateName);
            return Template.Parse(template);
        }
        public Template GetOverdueAdminsTemplateForMattermost()
        {
            var template = File.ReadAllText(_mattermostNotifierConfig.Value.AdminNotificationTemplateName);
            return Template.Parse(template);
        }
        public Template GetDeletedUsersTemplateForMattermost()
        {
            var template = File.ReadAllText(_mattermostNotifierConfig.Value.DeletedUserNotificationTemplateName);
            return Template.Parse(template);
        }
        public Template GetDeletedUsersTemplateForEmail()
        {
            var template = File.ReadAllText(_emailNotifierConfig.Value.DeletedUserNotificationTemplateName);
            return Template.Parse(template);
        }
    }
}