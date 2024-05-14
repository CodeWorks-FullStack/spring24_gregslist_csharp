
namespace csharp_gregslist_api.Services;

public class CarsService
{
  private readonly CarsRepository _repository;

  public CarsService(CarsRepository repository)
  {
    _repository = repository;
  }

  internal Car CreateCar(Car carData)
  {
    Car car = _repository.CreateCar(carData);
    return car;
  }

  internal string DestroyCar(int carId, string userId)
  {
    Car carToDestroy = GetCarById(carId);

    if (carToDestroy.CreatorId != userId)
    {
      throw new Exception("NOT YOU CAR BUD");
    }

    _repository.DestroyCar(carId);

    return $"{carToDestroy.Make} {carToDestroy.Model} has been deleted!";
  }

  internal Car GetCarById(int carId)
  {
    Car car = _repository.GetCarById(carId);

    if (car == null)
    {
      throw new Exception($"Invalid id: {carId}");
    }

    return car;
  }

  internal List<Car> GetCars()
  {
    List<Car> cars = _repository.GetCars();
    return cars;
  }

  internal Car UpdateCar(int carId, string userId, Car carData)
  {

    Car carToUpdate = GetCarById(carId);  // Get original car out of database

    // Check ownership
    if (carToUpdate.CreatorId != userId)
    {
      throw new Exception("You are not the creator of this car, bud");
    }

    // Changes data on car from database depending on if a value is supplied in the request body. If no value is supplied in the request body (null) it defaults to what it had stored originally
    carToUpdate.Make = carData.Make ?? carToUpdate.Make;
    carToUpdate.Model = carData.Model ?? carToUpdate.Model;

    // NOTE must make these properties nullable in the class model in order for this to work
    carToUpdate.Price = carData.Price ?? carToUpdate.Price;
    carToUpdate.HasCleanTitle = carData.HasCleanTitle ?? carToUpdate.HasCleanTitle;

    // update data in database
    Car updatedCar = _repository.UpdateCar(carToUpdate);

    return updatedCar;
  }
}