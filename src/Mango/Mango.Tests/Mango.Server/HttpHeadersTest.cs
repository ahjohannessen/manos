
using System;
using System.IO;

using NUnit.Framework;



namespace Mango.Server.Tests
{
	[TestFixture()]
	public class HttpHeadersTest
	{

		[Test()]
		public void TestCtor ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			Assert.IsNull (headers.ContentLength, "a1");
		}
		
		[Test()]
		public void TestSingleValueParse ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key: Value";
			
			headers.Parse (new StringReader (str));
			
			Assert.AreEqual ("Value", headers ["Key"], "a1");
			Assert.AreEqual (1, headers.Count, "a2");
		}
		
		[Test()]
		public void TestMultipleValueParse ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key1: Value1\nKey2: Value2\nKey3: Value3";
			
			headers.Parse (new StringReader (str));
			
			Assert.AreEqual ("Value1", headers ["Key1"], "a1");
			Assert.AreEqual ("Value2", headers ["Key2"], "a2");
			Assert.AreEqual ("Value3", headers ["Key3"], "a3");
			Assert.AreEqual (3, headers.Count, "a4");
		}
		
		[Test()]
		public void TestMultilineParse ()
		{
			//
			// multiline values are acceptable if the next 
			// line starts with spaces
			//
			string header = @"HeaderName: Some multiline
  								value";
		
			HttpHeaders headers = new HttpHeaders ();
			
			headers.Parse (new StringReader (header));
			
			Assert.AreEqual ("some multiline value", headers ["HeaderName"], "a1");
			
			header = @"HeaderName: Some multiline
  								value
	that spans
	a bunch of lines";
			
			headers = new HttpHeaders ();
			headers.Parse (new StringReader (header));
			
			Assert.AreEqual ("Some multiline value that spans a bunch of lines", headers ["HeaderName"], "a2");
		}
		
		[Test]
		public void TestParseNoValue ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key:\n";
			
			Assert.Throws<HttpException> (() => headers.Parse (new StringReader (str)));
			Assert.AreEqual (0, headers.Count, "a2");
			Assert.IsNull (headers.ContentLength, "a3");
		}
		
		[Test]
		public void TestParseNoColon ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key value";
			
			Assert.Throws<HttpException> (() => headers.Parse (new StringReader (str)));
			Assert.AreEqual (0, headers.Count, "a2");
			Assert.IsNull (headers.ContentLength, "a3");
		}
		
		[Test()]
		public void TestNormalizeNoDash ()
		{	
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("foo"));
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("FOO"));
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("FOo"));
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("foO"));
		}
		
		[Test()]
		public void TestNormalizeDashedName ()
		{
			Assert.AreEqual ("Foo-Bar", HttpHeaders.NormalizeName ("foo-bar"));
		}
	}
}