# University of Washington, Programming Languages, Homework 6, hw6runner.rb

# This is the only file you turn in, so do not modify the other files as
# part of your solution.

class MyPiece < Piece
  # The constant All_My_Pieces should be declared here
  All_My_Pieces = Piece::All_Pieces + \
  [
   rotations([[0, 0], [1, 0], [0, 1], [1, 1], [1,2]]), #inverted Square plus tail
   [[[0, 0], [-1, 0], [1, 0], [2, 0], [-2,0]],
    [[0, 0], [0, -1], [0, 1], [0, 2], [0,-2]]], # super long (only needs two)
   rotations([[0, 0], [1, 0], [0, 1]]) #square minus a block
  ]
  
  Cheat_Piece = [[[0,0]]]
  # your enhancements here
  def self.next_piece (board)
    if board.check_cheat
      MyPiece.new(Cheat_Piece, board)
    else 
      MyPiece.new(All_My_Pieces.sample, board)
    end
  end
end

class MyBoard < Board
  # your enhancements here

  #new constructor
  def initialize (game)
    @grid = Array.new(num_rows) {Array.new(num_columns)}
    @current_block = MyPiece.next_piece(self)
    @score = 0
    @game = game
    @delay = 500
    @cheat_state = false
  end

  def check_cheat
    @cheat_state
  end

  #new next_piece
  def next_piece
    @current_block = MyPiece.next_piece(self)
    @current_pos = nil
    @cheat_state = false
  end

  def cheat
    if !@cheat_state && @score >= 100
      @cheat_state = true
      @score = @score - 100
      @game.update_score
    end
  end

  def store_current
    locations = @current_block.current_rotation
    displacement = @current_block.position
    (0..(locations.size - 1)).each{|index| 
      current = locations[index];
      @grid[current[1]+displacement[1]][current[0]+displacement[0]] = 
      @current_pos[index]
    }
    remove_filled
    @delay = [@delay - 2, 80].max
  end
  
end

##To be honost, I don't like the design, it is very messy that the event bindings crossing each other.

class MyTetris < Tetris
  # your enhancements here
  def key_bindings
    super
    @root.bind('u', proc {@board.rotate_counter_clockwise; @board.rotate_counter_clockwise}) # key u binds to double rotate
    @root.bind('c', proc {@board.cheat}) #bind key c to cheat
  end

  def set_board
    @canvas = TetrisCanvas.new
    @board = MyBoard.new(self)
    @canvas.place(@board.block_size * @board.num_rows + 3,
                  @board.block_size * @board.num_columns + 6, 24, 80)
    @board.draw
  end

end


