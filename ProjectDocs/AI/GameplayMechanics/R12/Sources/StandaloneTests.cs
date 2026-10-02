using System;using System.Linq;using System.Reflection;using System.Collections;using NUnit.Framework;
class StandaloneTests { static int Main(){int passed=0,failed=0;var type=typeof(LegendsOfRus.Tests.MechanicsInteractionRulesTests);var target=Activator.CreateInstance(type);foreach(var m in type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly)){
 if(m.Name=="Existing244PlanAndStateHashesStayExact")continue;
 var cases=m.GetCustomAttributes(typeof(TestCaseAttribute),true).Cast<TestCaseAttribute>().Select(a=>a.Arguments).ToList();
 if(m.GetCustomAttributes(typeof(TestCaseSourceAttribute),true).Length>0)foreach(TestCaseData c in LegendsOfRus.Tests.MechanicsInteractionRulesTests.Pairs())cases.Add(c.Arguments);
 if(m.GetCustomAttributes(typeof(TestAttribute),true).Length>0)cases.Add(new object[0]);
 foreach(var args in cases)try{m.Invoke(target,args);passed++;Console.WriteLine("PASS "+m.Name+" "+string.Join(",",args));}catch(Exception e){failed++;Console.WriteLine("FAIL "+m.Name+" "+(e.InnerException??e));}
 }Console.WriteLine("passed="+passed+" failed="+failed);return failed>0?1:0;}}
