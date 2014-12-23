﻿using System;

namespace Kore.Web.ContentManagement.Areas.Admin.Widgets.RuleEngine
{
    public class DisabledRuleProvider : IRuleProvider
    {
        public void Process(RuleContext ruleContext)
        {
            if (!string.Equals(ruleContext.FunctionName, "disabled", StringComparison.OrdinalIgnoreCase))
                return;
            ruleContext.Result = false;
        }
    }
}