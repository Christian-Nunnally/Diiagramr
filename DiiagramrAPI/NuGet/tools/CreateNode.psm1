
function Create-Node {
	$csproj = (Get-ChildItem -Filter "*.csproj").FullName
	
	Write-Host "`nEnter new node name: " -NoNewline
	$nodeName = Read-Host
	
	$regex = "(?<=<AssemblyName>).*(?=<\/AssemblyName>)"
	$assembly = ""
	select-string -Path $csproj -Pattern $regex -AllMatches | % { $_.Matches } | % { $_.Value } -outvariable assembly
	
	$viewModelClassName = $nodeName + "ViewModel"
	$viewModelFile = $viewModelClassName + ".cs"
	New-Item $viewModelFile -type file -Force
	"using DiiagramrAPI.PluginNodeApi;" >> $viewModelFile
	"" >> $viewModelFile
	"namespace " + $assembly >> $viewModelFile
	"{" >> $viewModelFile
	"    public class " + $viewModelClassName + " : PluginNode" >> $viewModelFile
	"    {" >> $viewModelFile
	"        public override void SetupNode(NodeSetup setup)" >> $viewModelFile
	"        {" >> $viewModelFile
	"            setup.NodeSize(40, 40);" >> $viewModelFile
	"            setup.NodeName(`"" + $nodeName + "`");" >> $viewModelFile
	"        }" >> $viewModelFile
	"    }" >> $viewModelFile
	"}" >> $viewModelFile
	
	$viewClassName = $nodeName + "View"
	$viewFile = $viewClassName + ".xaml"
	New-Item $viewFile -type file -Force
	"<UserControl x:Class=`"" + $assembly + "." + $viewClassName + "`"" >> $viewFile
	"             xmlns=`"http://schemas.microsoft.com/winfx/2006/xaml/presentation`"" >> $viewFile
	"             xmlns:x=`"http://schemas.microsoft.com/winfx/2006/xaml`"" >> $viewFile
	"             xmlns:mc=`"http://schemas.openxmlformats.org/markup-compatibility/2006`"" >> $viewFile
	"             xmlns:d=`"http://schemas.microsoft.com/expression/blend/2008`"" >> $viewFile
	"             xmlns:s=`"https://github.com/canton7/Stylet`"" >> $viewFile
	"             xmlns:local=`"clr-namespace:" + $assembly + "`"" >> $viewFile
	"             d:DataContext=`"{d:DesignInstance local:" + $viewModelClassName + "}`"" >> $viewFile
	"             d:DesignHeight=`"40`"" >> $viewFile
	"             d:DesignWidth=`"40`"" >> $viewFile
	"             mc:Ignorable=`"d`">" >> $viewFile
	"</UserControl>" >> $viewFile
}

Export-ModuleMember Create-Node