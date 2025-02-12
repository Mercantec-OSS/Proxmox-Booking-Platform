public static class Helpers {
    public static int GetRandomNumber(int min = 10000, int max = 99999) {
        Random random = new Random();
        return random.Next(min, max);
    }
}