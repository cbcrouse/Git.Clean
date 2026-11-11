using FluentValidation.TestHelper;
using Git.Clean.Validators;
using Git.Clean.Commands;
using System.IO;
using System.Reflection;
using Xunit;

namespace Git.Clean.Tests
{
    public class RemoveStaleBranchesValidatorTests
    {
        [Fact]
        public void Validator_Fails_OnInvalidDirectory()
        {
            // Arrange
            const string invalidDirectory = "X:\\Temp";
            var sut = new RemoveStaleBranchesValidator();

            // Act & Assert
            sut.TestValidate(new RemoveStaleBranches { GitDirectory = invalidDirectory }).ShouldHaveValidationErrorFor(x => x.GitDirectory).WithErrorMessage("Must be a valid directory.");
        }

        [Fact]
        public void Validator_Fails_OnInvalidGitDirectory()
        {
            // Arrange
            string invalidDirectory = Path.GetTempPath();
            var sut = new RemoveStaleBranchesValidator();

            // Act & Assert
            sut.TestValidate(new RemoveStaleBranches { GitDirectory = invalidDirectory }).ShouldHaveValidationErrorFor(x => x.GitDirectory).WithErrorMessage("Must be a valid .git directory.");
        }

        [Fact]
        public void Validator_Succeeds_OnValidDirectory()
        {
            // Arrange
            string frameworkDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName;
            string configDir = Directory.GetParent(frameworkDir)!.FullName;
            string binDir = Directory.GetParent(configDir)!.FullName;
            string projectDir = Directory.GetParent(binDir)!.FullName;
            string tstDir = Directory.GetParent(projectDir)!.FullName;
            string slnDir = Directory.GetParent(tstDir)!.FullName;
            var sut = new RemoveStaleBranchesValidator();

            // Act & Assert
            sut.TestValidate(new RemoveStaleBranches { GitDirectory = slnDir }).ShouldNotHaveValidationErrorFor(x => x.GitDirectory);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Validator_Fails_OnInvalidStaleMonths(int staleMonths)
        {
            // Arrange
            var sut = new RemoveStaleBranchesValidator();

            // Act & Assert
            sut.TestValidate(new RemoveStaleBranches { StaleMonths = staleMonths }).ShouldHaveValidationErrorFor(x => x.StaleMonths);
        }

        [Fact]
        public void Validator_HasCorrectValidators()
        {
            // Arrange
            var sut = new RemoveStaleBranchesValidator();

            // Act & Assert
            sut.TestValidate(new RemoveStaleBranches { StaleMonths = 0 })
                .ShouldHaveValidationErrorFor(x => x.StaleMonths);
            sut.TestValidate(new RemoveStaleBranches { RemoteTarget = null! })
                .ShouldHaveValidationErrorFor(x => x.RemoteTarget);
            sut.TestValidate(new RemoveStaleBranches { RemoteTarget = string.Empty })
                .ShouldHaveValidationErrorFor(x => x.RemoteTarget);
            sut.TestValidate(new RemoveStaleBranches { IgnoreBranches = null! })
                .ShouldHaveValidationErrorFor(x => x.IgnoreBranches);
            sut.TestValidate(new RemoveStaleBranches())
                .ShouldNotHaveValidationErrorFor(x => x.StaleMonths);
            sut.TestValidate(new RemoveStaleBranches())
                .ShouldNotHaveValidationErrorFor(x => x.RemoteTarget);
            sut.TestValidate(new RemoveStaleBranches())
                .ShouldNotHaveValidationErrorFor(x => x.IgnoreBranches);
        }
    }
}