using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace FindTheWord
{
	public class GameScreen : Scene
	{
		public GameScreen()
		{
			currentLevelIndex = -1;
			randomizer = new PseudoRandom();
			SetViewportBackground("GameBackground");
			var font = ContentLoader.Load<Font>("Tahoma30");
			currentLevelFontText = new FontText(font, "", Rectangle.FromCenter(new Vector2D(0.5f, DrawArea.Top + 0.079f), new Size(0.6f,0.3f)));
			nextLevel = new NextLevelScreen();
			CreateImageContainersForRiddle();
			CreateDisplayCharacterButtons();
			CreateLevels();
		}

		private int currentLevelIndex;
		private readonly PseudoRandom randomizer;
		private readonly NextLevelScreen nextLevel;

		private void CreateImageContainersForRiddle()
		{
			image1 = CreateSprite(0.1075f, 0.3268f);
			image2 = CreateSprite(0.396f, 0.3246f);
			image3 = CreateSprite(0.691f, 0.3239f);
		}

		private Sprite image1;
		private Sprite image2;
		private Sprite image3;

		private static Sprite CreateSprite(float xPosition, float yPosition)
		{
			const float Dimension = 245.0f / 1280.0f;
			return new Sprite("Mouse1", new Rectangle(xPosition, yPosition, Dimension, Dimension))
			{
				RenderLayer = 2
			};
		}

		private void CreateDisplayCharacterButtons()
		{
			displayCharacters = new List<CharacterButton>();
			float xCenter = DisplayCharactersCenterStartX;
			float yCenter = 0.6935f;
			for (int i = 0; i < 12; i++)
			{
				if (i != 0 && i % 6 == 0)
				{
					xCenter = DisplayCharactersCenterStartX;
					yCenter += FullCharacterGap;
				}
				AddNewDisplayCharToList(xCenter, yCenter, i);
				xCenter += FullCharacterGap;
			}
		}

		private void AddNewDisplayCharToList(float xCenter, float yCenter, int index)
		{
			var newCharButton = new CharacterButton(xCenter, yCenter, this);
			newCharButton.Clicked += newCharButton.OnDisplayCharButtonClicked;
			newCharButton.Index = index;
			displayCharacters.Add(newCharButton);
		}

		private List<CharacterButton> displayCharacters;
		private const float DisplayCharactersCenterStartX = 0.378f;
		private const float FullCharacterGap = CharacterButton.Dimension + 0.00025f;

		public void OnDisplayCharButtonClicked(CharacterButton displayCharButton)
		{
			var freeButton = FindNextFreeSolutionButton();
			if (freeButton == null)
				return;
			freeButton.Letter = displayCharButton.Letter;
			freeButton.Index = displayCharButton.Index;
			displayCharButton.RemoveLetter();
			if (IsWordCorrect())
				CompleteLevel();
		}

		private CharacterButton FindNextFreeSolutionButton()
		{
			foreach (CharacterButton button in solutionCharacters)
				if (button.Letter == CharacterButton.NoCharacter)
					return button;
			return null;
		}

		public new void Hide()
		{
			base.Hide();
			image1.IsVisible = false;
			image2.IsVisible = false;
			image3.IsVisible = false;
			foreach (CharacterButton character in displayCharacters)
				character.IsVisible = false;
		}

		public void FadeIn()
		{
			StartNextLevel();
			Show();
			image1.IsVisible = true;
			image2.IsVisible = true;
			image3.IsVisible = true;
			foreach (CharacterButton character in displayCharacters)
				character.IsVisible = true;
		}

		public void CompleteLevel()
		{
			levelComplete = true;
			ClearLevel();
			nextLevel.ShowAndWaitForInput();
			nextLevel.StartNextLevel += () => AdvanceToNextLevel();
		}

		private bool levelComplete;

		private void AdvanceToNextLevel()
		{
			if (!levelComplete)
				return;
			StartNextLevel();
			UpdateLevelFontText();
		}

		public void StartNextLevel()
		{
			currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
			currentRiddle = levels[currentLevelIndex];
			SetImagesToCurrentLevel();
			SetDisplayCharactersToCurrentLevel();
			CreateSolutionCharacterButtons();
			UpdateLevelFontText();
			levelComplete = false;
		}

		private void SetImagesToCurrentLevel()
		{
			image1.IsVisible = true;
			image2.IsVisible = true;
			image3.IsVisible = true;
			image1.Material = new Material(ShaderFlags.Position2DTextured, currentRiddle.Image1);
			image2.Material = new Material(ShaderFlags.Position2DTextured, currentRiddle.Image2);
			image3.Material = new Material(ShaderFlags.Position2DTextured, currentRiddle.Image3);
		}

		private void SetDisplayCharactersToCurrentLevel()
		{
			List<char> wordPlusFillCharacters = GetWordPlusFillCharacters();
			for (int i = 0; i < displayCharacters.Count; i++)
				displayCharacters[i].Letter = wordPlusFillCharacters[i];
		}

		private LevelData currentRiddle;

		private List<char> GetWordPlusFillCharacters()
		{
			var list = new List<char>(currentRiddle.SearchedWord.ToUpper().ToCharArray());
			while (list.Count < displayCharacters.Count)
				list.Add(GetRandomUpperCaseLetter());
			list = RandomizeList(list);
			return list;
		}

		private char GetRandomUpperCaseLetter()
		{
			int randomCharIndex = randomizer.Get('A', 'Z');
			return (char)randomCharIndex;
		}

		private List<char> RandomizeList(List<char> list)
		{
			var randomizedList = new List<char>();
			while (list.Count > 0)
			{
				int randomSelectionIndex = randomizer.Get(0, list.Count);
				randomizedList.Add(list[randomSelectionIndex]);
				list.RemoveAt(randomSelectionIndex);
			}
			return randomizedList;
		}

		private void CreateSolutionCharacterButtons()
		{
			if (solutionCharacters != null)
				foreach (CharacterButton character in solutionCharacters)
					character.Clicked -= character.OnSolutionCharButtonClicked;
			solutionCharacters = new List<CharacterButton>();
			int numberOfCharacters = currentRiddle.SearchedWord.Length;
			float xCenter = GetSolutionCharacterCenterStartX(numberOfCharacters);
			const float YCenter = 0.6f;
			const float ButtonGap = CharacterButton.Dimension + 0.00025f;
			for (int i = 0; i < numberOfCharacters; i++)
			{
				AddNewSolutionButtonToList(xCenter, YCenter, i);
				xCenter += ButtonGap;
			}
		}

		private void AddNewSolutionButtonToList(float xCenter, float yCenter, int index)
		{
			CharacterButton newCharButton = new CharacterButton(xCenter, yCenter, this);
			newCharButton.Clicked += newCharButton.OnSolutionCharButtonClicked;
			newCharButton.Index = index;
			solutionCharacters.Add(newCharButton);
		}

		private List<CharacterButton> solutionCharacters;

		public void OnSolutionCharButtonClicked(CharacterButton solutionCharButton)
		{
			var displayCharButton = displayCharacters[solutionCharButton.Index];
			solutionCharButton.Letter = CharacterButton.NoCharacter;
			displayCharButton.ShowLetter();
		}

		private static float GetSolutionCharacterCenterStartX(int numberOfCharacters)
		{
			const float FourCharStartPos = DisplayCharactersCenterStartX + FullCharacterGap;
			return numberOfCharacters == 4 ? FourCharStartPos : FourCharStartPos - FullCharacterGap / 2;
		}

		private void UpdateLevelFontText()
		{
			if (levelComplete)
			{
				currentLevelFontText.IsVisible = false;
				return;
			}
			currentLevelFontText.IsVisible = true;
			currentLevelFontText.Text = "Level " + (currentLevelIndex + 1);
		}

		private readonly FontText currentLevelFontText;

		private void CreateLevels()
		{
			levels = new List<LevelData>();
			levels.Add(CreateLevelData("Wurm", "WORM"));
			levels.Add(CreateLevelData("Mouse", "MOUSE"));
			levels.Add(CreateLevelData("Sea", "WATER"));
		}

		private List<LevelData> levels;

		private static LevelData CreateLevelData(string imageBaseName, string searchedWord)
		{
			return new LevelData
			{
				Image1 = imageBaseName + 1,
				Image2 = imageBaseName + 2,
				Image3 = imageBaseName + 3,
				SearchedWord = searchedWord,
			};
		}

		public override void Clear()
		{
			base.Clear();
			ClearLevel();
		}

		private void ClearSolutionChars()
		{
			if (solutionCharacters == null)
				return;
			foreach (var solutionCharacter in solutionCharacters)
				solutionCharacter.Dispose();
			solutionCharacters.Clear();
		}

		private bool IsWordCorrect()
		{
			for (int i = 0; i < solutionCharacters.Count; i++)
				if (!solutionCharacters[i].Letter.Equals(levels[currentLevelIndex].SearchedWord[i]))
					return false;
			return true;
		}

		private void ClearLevel()
		{
			ClearSolutionChars();
			image1.IsVisible = false;
			image2.IsVisible = false;
			image3.IsVisible = false;
		}
	}
}