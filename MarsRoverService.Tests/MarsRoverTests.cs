using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MarsRoverService.Tests;

public class MarsRoverTests
{
    private Rover _rover_R01;
    private Rover _rover_R02;
    private Plateau _plateau;
    private CommandCenter _commandCenter;

    private const int _GRID_MAX_X_COORDINATE = 5;
    private const int _GRID_MAX_Y_COORDINATE = 5;

    [SetUp]
    public void Setup()
    {
        //Creating an instance and setting up the plateau grid size
        _plateau = new Plateau();
        _plateau.SetPlateauGridSize(_GRID_MAX_X_COORDINATE, _GRID_MAX_Y_COORDINATE);

        //Creating instances for 2 Rovers - "Rover R01", "Rover R02"
        _rover_R01 = new Rover(_plateau);
        _rover_R02 = new Rover(_plateau);

        //Creating instance for CommandCenter
        _commandCenter = new CommandCenter();        
    }

    [Test]
    public void Test_If_Plateau_Grid_Size_Is_Set_As_Expected()
    {
        _plateau.pointGridMax.X.Should().Be(_GRID_MAX_X_COORDINATE);
        _plateau.pointGridMax.Y.Should().Be(_GRID_MAX_Y_COORDINATE);
    }

    [Test]
    public void Test_If_Plateau_Grid_Size_Can_Be_Set_As_A_Rectangle()
    {
        _plateau.SetPlateauGridSize(3, 4);
        _plateau.pointGridMax.X.Should().Be(3);
        _plateau.pointGridMax.Y.Should().Be(4);
    }

    [Test]
    public void Test_If_Plateau_Grid_Size_Can_Be_Set_As_A_Square()
    {
        _plateau.SetPlateauGridSize(3, 3);
        _plateau.pointGridMax.X.Should().Be(3);
        _plateau.pointGridMax.Y.Should().Be(3);
    }

    [Test]
    public void Test_If_Plateau_Grid_Size_Is_Valid()
    {
        var exception = Assert.Throws<ArgumentException>(() => _plateau.SetPlateauGridSize(-1, 0));
        Assert.That(exception.Message, Is.EqualTo("Plateau grid coordinates can't be negative. Please enter a valid plateau grid size."));

        var exceptionGridSize = Assert.Throws<ArgumentException>(() => _plateau.SetPlateauGridSize(0, 0));
        Assert.That(exceptionGridSize.Message, Is.EqualTo("The plateau grid size must be greater than (0, 0)"));
    }

    [Test]
    public void Test_If_Rover_Coordinates_And_Direction_Are_Set_As_Expected()
    {
        //1st Rover
        _rover_R01.SetRoverPosition(1, 2, 'N');
        _rover_R01.pointCurrent.X.Should().Be(1);
        _rover_R01.pointCurrent.Y.Should().Be(2);
        _rover_R01.CurrentDirectionFacing.Should().Be(Direction.North);

        //2nd Rover
        _rover_R02.SetRoverPosition(3, 3, 'E');
        _rover_R02.pointCurrent.X.Should().Be(3);
        _rover_R02.pointCurrent.Y.Should().Be(3);
        _rover_R02.CurrentDirectionFacing.Should().Be(Direction.East);
    }

    [Test]
    public void Test_If_Rover_Coordinates_Are_Valid()
    {
        var exception = Assert.Throws<ArgumentException>(() => _rover_R01.SetRoverPosition(-1, 2, 'N'));
        Assert.That(exception.Message, Is.EqualTo("Rover coordinates can't be negative. Please enter a valid Rover position."));

        var exceptionGridSize = Assert.Throws<ArgumentException>(() => _rover_R02.SetRoverPosition(3, -7, 'E'));
        Assert.That(exceptionGridSize.Message, Is.EqualTo("Rover coordinates can't be negative. Please enter a valid Rover position."));
    }

    [Test]
    public void Test_If_Rover_Direction_Given_In_Lowercase_Is_Accepted_As_Input_And_The_Rover_Is_Set_As_Expected()
    {
        //1st Rover
        _rover_R01.SetRoverPosition(1, 2, 'n');
        _rover_R01.pointCurrent.X.Should().Be(1);
        _rover_R01.pointCurrent.Y.Should().Be(2);
        _rover_R01.CurrentDirectionFacing.Should().Be(Direction.North);

        //2nd Rover
        _rover_R02.SetRoverPosition(3, 3, 'e');
        _rover_R02.pointCurrent.X.Should().Be(3);
        _rover_R02.pointCurrent.Y.Should().Be(3);
        _rover_R02.CurrentDirectionFacing.Should().Be(Direction.East);
    }

    [Test]
    public void Test_If_Rover_Is_Placed_Within_The_Palteau_As_Expected()
    {
        //1st Rover
        var exceptionR01Position = Assert.Throws<ArgumentException>(() 
            => _rover_R01.SetRoverPosition(5, 6, 'N'));
        Assert.That(exceptionR01Position.Message, Is.EqualTo("Rover position should not be outside the plateau grid."));

        //2nd Rover
        var exceptionR02Position = Assert.Throws<ArgumentException>(()
            => _rover_R02.SetRoverPosition(7, 5, 'N'));
        Assert.That(exceptionR02Position.Message, Is.EqualTo("Rover position should not be outside the plateau grid."));
    }

    [Test]
    public void Test_If_Rover_Facing_Direction_After_Move_Is_Set_As_Expected()
    {   
        //1st Rover
        _rover_R01.SetRoverPosition(1, 2, 'N');
        _commandCenter.MoveRover(_rover_R01, "LMLMLmlMM");
        _commandCenter.CurrentDirectionFacing.Should().Be(Direction.North);

        //2nd Rover
        _rover_R02.SetRoverPosition(3, 3, 'E');
        _commandCenter.MoveRover(_rover_R02, "MMRMMRMRrm");
        _commandCenter.CurrentDirectionFacing.Should().Be(Direction.East);
    }

    [Test]
    public void Test_If_The_Instructions_Make_Rover_Move_Out_Of_The_Plateau_And_Throw_Exception()
    {
        //1st Rover
        _rover_R01.SetRoverPosition(1, 2, 'N');
        var exceptionR01Position = Assert.Throws<ArgumentException>(()
            => _commandCenter.MoveRover(_rover_R01, "LMLMLMLMMLMM"));
        Assert.That(exceptionR01Position.Message, Is.EqualTo("Rover cannot move outside the plateau. " +
            "It now stands at the position (0, 3) facing West. Please modify the instructions."));

        //2nd Rover
        _rover_R02.SetRoverPosition(3, 3, 'E');
        var exceptionR02Position = Assert.Throws<ArgumentException>(()
            => _commandCenter.MoveRover(_rover_R02, "MMRMMRMRRMMM"));
        Assert.That(exceptionR02Position.Message, Is.EqualTo("Rover cannot move outside the plateau. " +
            "It now stands at the position (5, 1) facing East. Please modify the instructions."));

    }

    [Test]
    public void Test_If_Rover_Moves_And_Returns_Position_And_Direction_As_Expected()
    {
        //1st Rover
        _rover_R01.SetRoverPosition(1, 2, 'N');
        _commandCenter.MoveRover(_rover_R01, "LMLMLMLMM").Should().Be("1 3 N");

        //2nd Rover
        _rover_R02.SetRoverPosition(3, 3, 'E');
        _commandCenter.MoveRover(_rover_R02, "MMRMMRMRRM").Should().Be("5 1 E");        
    }

    [Test]
    public void Test_If_Plateau_Can_Be_Added_By_The_Command_Center()
    {      
        Plateau _newPlateau = new();
        _newPlateau = _commandCenter.AddPlateau(6, 6);
        _newPlateau.pointGridMax.X.Should().Be(6);
        _newPlateau.pointGridMax.Y.Should().Be(6);
    }

    [Test]
    public void Test_If_Rover_Can_Be_Added_By_The_Command_Center()
    {
        Plateau _newPlateau = new();
        _newPlateau = _commandCenter.AddPlateau(6, 6);
        Rover _newRover = new(_newPlateau);
        _newRover = _commandCenter.AddRover(1, 2, 'N');
        _newRover.pointCurrent.X.Should().Be(1);
        _newRover.pointCurrent.Y.Should().Be(2);
        _newRover.CurrentDirectionFacing.Should().Be(Direction.North);
    }

    [Test]
    public void Test_How_Many_Number_Of_Rovers_Can_Be_Added_By_The_Command_Center()
    {
        Plateau _newPlateau = new();
        _newPlateau = _commandCenter.AddPlateau(6, 4);
        Rover _newRover = new(_newPlateau);
        _newRover = _commandCenter.AddRover(1, 2, 'N');
        _commandCenter.PossibleNoOfRovers.Should().Be(12);
    }

    [Test]
    public void Test_If_Rover_Can_Be_Added_And_Moved_By_The_Command_Center()
    {
        Plateau _newPlateau = new();
        _newPlateau = _commandCenter.AddPlateau(6, 6);
        Rover _newRover = new(_newPlateau);
        _newRover = _commandCenter.AddRover(1, 2, 'N');
        _commandCenter.MoveRover(_newRover, "LMLMLMLMM").Should().Be("1 3 N");
    }
    
    [Test]
    public void Test_If_2_Rovers_Can_Be_Added_On_The_Plateau_And_Moved_By_The_Command_Center()
    {
        Plateau _newPlateau = new();
        _newPlateau = _commandCenter.AddPlateau(6, 6);

        //Rover 1
        Rover R01 = new(_newPlateau);
        R01 = _commandCenter.AddRover(1, 2, 'N');
        _commandCenter.MoveRover(R01, "LMLMLMLMM");

        //Rover 2
        Rover R02 = new(_newPlateau);
        R02 = _commandCenter.AddRover(3, 3, 'E');
        _commandCenter.MoveRover(R02, "MMRMMRMRRM");

        List<Rover> rovers = new List<Rover>();
        rovers = _commandCenter.GetRoversList().ToList<Rover>();

        rovers[0].pointCurrent.X.Should().Be(1);
        rovers[0].pointCurrent.Y.Should().Be(3);
        rovers[0].CurrentDirectionFacing.Should().Be(Direction.North);

        rovers[1].pointCurrent.X.Should().Be(5);
        rovers[1].pointCurrent.Y.Should().Be(1);
        rovers[1].CurrentDirectionFacing.Should().Be(Direction.East);
    }
    
    [Test]
    public void Test_For_Collision_If_2_Rovers_Are_Added_On_The_Plateau_And_Moved_By_The_Command_Center_And_Throw_Exception()
    {
        Plateau _newPlateau = new();
        _newPlateau = _commandCenter.AddPlateau(5, 5);

        //Rover 1
        Rover R01 = new(_newPlateau);
        R01 = _commandCenter.AddRover(4, 0, 'S');
        _commandCenter.MoveRover(R01, "LMLM"); 

        //Rover 2
        Rover R02 = new(_newPlateau);
        R02 = _commandCenter.AddRover(4, 2, 'N');        
        var exceptionR02Position = Assert.Throws<ArgumentException>(()
            => _commandCenter.MoveRover(R02, "RMRM"));
        Assert.That(exceptionR02Position.Message, Is.EqualTo("Rover cannot move further. " +
            "There is a collision ahead. It now stands at the position (5, 2) facing South. " +
            "Please modify the instructions."));
    }

    [Test]
    public void Test_For_Collision_If_3_Rovers_Are_Added_On_The_Plateau_And_Moved_By_The_Command_Center_And_Throw_Exception()
    {
        Plateau _newPlateau = new();
        _newPlateau = _commandCenter.AddPlateau(5, 5);

        //Rover 1
        Rover R01 = new(_newPlateau);
        R01 = _commandCenter.AddRover(4, 0, 'S');
        _commandCenter.MoveRover(R01, "LMLM");

        //Rover 2
        Rover R02 = new(_newPlateau);
        R02 = _commandCenter.AddRover(4, 2, 'N');
        _commandCenter.MoveRover(R02, "RM");

        //Rover 3
        Rover R03 = new(_newPlateau);
        R02 = _commandCenter.AddRover(4, 3, 'N');
        var exceptionR02Position = Assert.Throws<ArgumentException>(()
            => _commandCenter.MoveRover(R02, "RMRM"));
        Assert.That(exceptionR02Position.Message, Is.EqualTo("Rover cannot move further. " +
            "There is a collision ahead. It now stands at the position (5, 3) facing South. " +
            "Please modify the instructions."));
    }
}