using Is.Assertions;
using Is.Core;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class FileTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[Test]
	public void Files_IsExisting()
	{
		var tempFile = Path.GetTempFileName();
		var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		Directory.CreateDirectory(tempDir);

		try
		{
			tempFile.IsExisting();
			tempDir.IsExisting();

			Action action = () => "/nonexistent/path/that/does/not/exist".IsExisting();
			action.IsThrowing<NotException>("does not exist");
		}
		finally
		{
			File.Delete(tempFile);
			Directory.Delete(tempDir);
		}
	}
}
