
namespace csharp_gregslist_api.Repositories;

public class CarsRepository
{
  private readonly IDbConnection _db;

  public CarsRepository(IDbConnection db)
  {
    _db = db;
  }

  internal Car CreateCar(Car carData)
  {
    string sql = @"
    INSERT INTO
    cars(
      make,
      model,
      year,
      price,
      imgUrl,
      description,
      engineType,
      color,
      mileage,
      hasCleanTitle,
      creatorId
    )
    VALUES(
      @Make,
      @Model,
      @Year,
      @Price,
      @ImgUrl,
      @Description,
      @EngineType,
      @Color,
      @Mileage,
      @HasCleanTitle,
      @CreatorId
    );
    
    SELECT 
    cars.*,
    accounts.*
    FROM cars
    JOIN accounts ON accounts.id = cars.creatorId
    WHERE cars.id = LAST_INSERT_ID();";

    Car car = _db.Query<Car, Account, Car>(sql, (car, account) =>
    {
      car.Creator = account;
      return car;
    }, carData).FirstOrDefault();

    return car;
  }

  internal void DestroyCar(int carId)
  {
    string sql = "DELETE FROM cars WHERE id = @carId;";

    _db.Execute(sql, new { carId });
  }

  internal Car GetCarById(int carId)
  {
    string sql = @"
    SELECT 
    cars.*,
    accounts.* 
    FROM cars 
    JOIN accounts ON accounts.id = cars.creatorId
    WHERE cars.id = @carId;";

    Car car = _db.Query<Car, Account, Car>(sql, (car, account) =>
    {
      car.Creator = account;
      return car;
    }, new { carId }).FirstOrDefault();

    return car;
  }

  internal List<Car> GetCars()
  {
    string sql = @"
    SELECT 
    cars.*,
    accounts.* 
    FROM cars
    JOIN accounts ON accounts.id = cars.creatorId;";

    //                          | First data type coming on each row 
    //                          |       | Second data type coming in on each row
    //                          |       |     | return type for mapping function
    //                          V       V     V
    List<Car> cars = _db.Query<Car, Account, Car>(sql, (car, account) =>
    {
      // Assigning creator object to car object. Dapper will pass each data type coming in on each row to this mapping function
      car.Creator = account;
      return car;
    }).ToList();

    return cars;
  }

  internal Car UpdateCar(Car carToUpdate)
  {
    string sql = @"
    UPDATE cars
    SET
    make = @Make,
    model = @Model,
    price = @Price,
    hasCleanTitle = @HasCleanTitle
    WHERE id = @Id;
    
    SELECT
    cars.*,
    accounts.*
    FROM cars
    JOIN accounts ON accounts.id = cars.creatorId
    WHERE cars.id = @Id;";

    Car car = _db.Query<Car, Account, Car>(sql, (car, account) =>
    {
      car.Creator = account;
      return car;
    }, carToUpdate).FirstOrDefault();

    return car;
  }
}