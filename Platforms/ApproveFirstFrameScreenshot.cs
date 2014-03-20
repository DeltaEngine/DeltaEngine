using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Causes a unit test to take a screenshot after the first frame and verify if the current 
	/// unit test run matches previous ones.
	/// </summary>
	public abstract class ApproveFirstFrameScreenshot : Resolver
	{
		//ncrunch: no coverage start
		protected ApproveFirstFrameScreenshot()
		{
			if (!StackTraceExtensions.IsStartedFromNCrunch() && StackTraceExtensions.IsUnitTest() &&
					ExcludeSharpDXAsScreenshotsCanOnlyBeMadeDelayed())
				CheckApprovalImageAfterFirstFrame();
		}

		/// <summary>
		/// For details see SharpDXScreenshotCapturer, all other frameworks work fine
		/// </summary>
		private bool ExcludeSharpDXAsScreenshotsCanOnlyBeMadeDelayed()
		{
			return !GetType().Name.Contains("SharpDX");
		}

		public override void Dispose()
		{
			if (madeApprovalImage)
				CheckApprovalTestResult();
			base.Dispose();
		}

		private bool madeApprovalImage;

		private void CheckApprovalImageAfterFirstFrame()
		{
			var approvalTestName = StackTraceExtensions.GetApprovalTestName();
			if (String.IsNullOrEmpty(approvalTestName))
				return;
			firstFrameApprovalImageFilename = "..\\..\\" + approvalTestName + "." +
				GetType().Name.Replace("Resolver", "") + ".png";
		}

		private string firstFrameApprovalImageFilename;

		protected void ExecuteTestCodeAndMakeScreenshotAfterFirstFrame()
		{
			if (CodeAfterFirstFrame != null)
				CodeAfterFirstFrame();
			CodeAfterFirstFrame = null;
			if (madeApprovalImage || !HasApprovalImage)
				return;
			madeApprovalImage = true;
			Resolve<ScreenshotCapturer>().MakeScreenshot(firstFrameApprovalImageFilename);
		}

		public Action CodeAfterFirstFrame;

		private bool HasApprovalImage
		{
			get { return !String.IsNullOrEmpty(firstFrameApprovalImageFilename); }
		}

		private void CheckApprovalTestResult()
		{
			if (!File.Exists(firstFrameApprovalImageFilename))
				throw new FileNotFoundException(
					"Unable to approve test as no image was generated and saved after the first frame.",
					firstFrameApprovalImageFilename);
			string extension = "." + GetType().Name.Replace("Resolver", "");
			approvedImageFileName = firstFrameApprovalImageFilename.Replace(extension, ".approved");
			if (File.Exists(approvedImageFileName))
				CompareImages();
			else
				UseFirstFrameImageAsApprovedImage();
		}

		private string approvedImageFileName;

		private void CompareImages()
		{
			float difference = 0.0f;
			using (var approvedBitmap = new Bitmap(approvedImageFileName))
			using (var compareBitmap = new Bitmap(firstFrameApprovalImageFilename))
				if (approvedBitmap.Width != compareBitmap.Width ||
					approvedBitmap.Height != compareBitmap.Height)
					ImagesAreDifferent("Approved image size " + approvedBitmap.Width + "x" +
						approvedBitmap.Height + " is different from the compare image size: " +
						compareBitmap.Width + "x" + compareBitmap.Height);
				else
					difference = CompareImageContent(approvedBitmap, compareBitmap);
			if (difference < MaxAllowedImageDifference)
				ApprovalTestSucceeded(difference);
			else
				ImagesAreDifferent("Difference " + difference.ToString("0.00") + "%");
		}

		private const float MaxAllowedImageDifference = 0.99f;

		private void ImagesAreDifferent(string errorMessage)
		{
			errorMessage = "Approval test failed with " + GetType().Name + ", resulting image is " +
				"different from the approved one: " + Path.GetFileName(firstFrameApprovalImageFilename) +
				"\n" + errorMessage;
			LaunchTortoiseIDiffIfAvailable();
			throw new ImageCompareFailed(errorMessage);
		}

		private void LaunchTortoiseIDiffIfAvailable()
		{
			if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				return;
			if (File.Exists(TortoiseIDiffFilePath))
				Process.Start(TortoiseIDiffFilePath,
					"/left:\"" + approvedImageFileName + "\" /right:\"" + firstFrameApprovalImageFilename +
						"\" /showinfo");
			else
			{
				Process.Start(approvedImageFileName);
				Process.Start(firstFrameApprovalImageFilename);
			}
		}

		private const string TortoiseIDiffFilePath =
			@"C:\Program Files\TortoiseSVN\bin\TortoiseIDiff.exe";

		private class ImageCompareFailed : Exception
		{
			public ImageCompareFailed(string errorMessage)
				: base(errorMessage) {}
		}

		private static unsafe float CompareImageContent(Bitmap approvedBitmap, Bitmap compareBitmap)
		{
			var approvedData = GetBitmapData(approvedBitmap);
			var compareData = GetBitmapData(compareBitmap);
			float difference = GetImageDifference(approvedBitmap.Width, approvedBitmap.Height,
				(byte*)approvedData.Scan0.ToPointer(), (byte*)compareData.Scan0.ToPointer());
			approvedBitmap.UnlockBits(approvedData);
			compareBitmap.UnlockBits(compareData);
			return difference;
		}

		private void ApprovalTestSucceeded(float difference)
		{
			if (difference > MaxAllowedImageDifference / 9)
			{
				Console.WriteLine(
					"Warning: Screenshot is almost equal to approval image, but not pixel perfect, check " +
					"if the image is still okay.\nKeeping " + firstFrameApprovalImageFilename + " around " +
					"for manual comparison.\nDifference to approved image: " + difference.ToString("0.00"));
				return;
			}
			Console.WriteLine("Approval test succeeded, difference to approved image: " +
				difference.ToString("0.00") + "%\nDeleting " +
				Path.GetFileName(firstFrameApprovalImageFilename));
			File.Delete(firstFrameApprovalImageFilename);
		}

		private static BitmapData GetBitmapData(Bitmap bitmap)
		{
			return bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
		}

		private static unsafe float GetImageDifference(int width, int height, byte* approvedBytes,
			byte* compareBytes)
		{
			float totalDifference = 0.0f;
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
				{
					int index = (y * width + x) * 3;
					int blue = compareBytes[index] - approvedBytes[index];
					int green = compareBytes[index + 1] - approvedBytes[index + 1];
					int red = compareBytes[index + 2] - approvedBytes[index + 2];
					int difference = blue * 12 + green * 58 + red * 30;
					if (difference < 0)
						totalDifference -= difference / 255.0f;
					else
						totalDifference += difference / 255.0f;
				}
			return totalDifference / (width * height);
		}

		private void UseFirstFrameImageAsApprovedImage()
		{
			File.Move(firstFrameApprovalImageFilename, approvedImageFileName);
			Console.WriteLine(Path.GetFileName(approvedImageFileName) +
				" did not exist and was created from this test. Run again to compare the result.");
		}
	}
}